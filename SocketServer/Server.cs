using SocketServer.Models;
using SocketServer.Protocol;
using SocketServer.Protocol.DTOs;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace SocketServer
{
    public class Server
    {
        GameServer mainGameServer;

        Socket mainSocket;
        int m_port = 5000;

        // roomId, Socket
        Dictionary<Guid, Socket> connectedClients = null;

        public void ServerStart()
        {
            mainGameServer = new GameServer();

            connectedClients = new Dictionary<Guid, Socket>();

            mainSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            mainSocket.Bind(new IPEndPoint(IPAddress.Any, m_port));
            mainSocket.Listen(100);

            // IOCP-EAP Pattern
            SocketAsyncEventArgs sockAsync = new SocketAsyncEventArgs();
            sockAsync.Completed += new EventHandler<SocketAsyncEventArgs>(SockAsync_NewClient);
            mainSocket.AcceptAsync(sockAsync);
        }

        private void SockAsync_NewClient(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                Socket server = (Socket)sender;
                Socket client = e.AcceptSocket;

                SocketAsyncEventArgs received = new SocketAsyncEventArgs();
                received.Completed += new EventHandler<SocketAsyncEventArgs>(SockAsync_DataReceived);
                received.SetBuffer(new byte[2048], 0, 2048);
                received.UserToken = client;
                client.ReceiveAsync(received);

                e.AcceptSocket = null;
                server.AcceptAsync(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void SockAsync_DataReceived(object sender, SocketAsyncEventArgs e)
        {
            Socket client = (Socket)sender;

            if (client.Connected && e.BytesTransferred > 0)
            {
                Console.WriteLine("Some Data From Client! ====================");
                int length = e.BytesTransferred;
                byte[] buffer = new byte[length];
                Array.Copy(e.Buffer, buffer, length);

                CPacketType route = (CPacketType)buffer[0];
                Console.WriteLine("packet type: " + route.ToString());
                byte[] data = null;
                if (length > 0)
                {
                    data = new byte[length - 1];
                    Array.Copy(buffer, 1, data, 0, length - 1);
                }

                // MIGHTDO: Dictionary 도입 고려!
                switch (route)
                {
                    case CPacketType.I_ROOMLIST:
                        Game_RoomList(client);
                        break;
                    case CPacketType.I_CREATEROOM:
                        Game_CreateRoom(data, client);
                        break;
                    case CPacketType.I_JOINROOM:
                        Game_JoinRoom(data, client);
                        break;
                    case CPacketType.L_STATUS:
                        Game_StatusChange(data);
                        break;
                    case CPacketType.L_LEAVE:
                        Game_LeaveRoom(data);
                        break;
                    case CPacketType.L_START:
                        Game_StartRoom(data);
                        break;
                    case CPacketType.F_MVE:
                        Game_PlayerMove(data);
                        break;
                }

                client.ReceiveAsync(e);
            }
        }

        public void Close()
        {
            if (mainSocket != null)
            {
                mainSocket.Close();
                mainSocket.Dispose();
            }
            foreach (var s in connectedClients)
            {
                s.Value.Close();
                s.Value.Dispose();
            }
            connectedClients.Clear();
        }

        #region Game Logics
        // CPacketType.I_ROOMLIST
        private void Game_RoomList(Socket client)
        {
            byte[] dataToSend = SerializeData(mainGameServer.GetRooms(), SPacketType.C_ROOMLIST);
            client.Send(dataToSend);
        }
        // CPacketType.I_CREATEROOM
        private void Game_CreateRoom(byte[] data, Socket client)
        {
            CreateRoomReqDto newRoom = JsonSerializer.Deserialize<CreateRoomReqDto>(data);
            MGameRoom room = mainGameServer.CreateRoom(newRoom.RoomNm);
            MPlayer mPlayer = new MPlayer(newRoom.PlayerNm);
            mainGameServer.JoinRoom(room.RoomId, mPlayer);

            connectedClients.Add(mPlayer.PlayerId, client);

            LobbyResDto lobbyResDto = new LobbyResDto()
            {
                PlayerId = mPlayer.PlayerId,
                Room = room
            };
            byte[] dataToSend = SerializeData(lobbyResDto, SPacketType.C_ENTERLOBBY);
            client.Send(dataToSend);
        }

        // CPacketType.I_JOINROOM
        private void Game_JoinRoom(byte[] data, Socket client)
        {
            JoinRoomReqDto joinRoom = JsonSerializer.Deserialize<JoinRoomReqDto>(data);

            MPlayer mPlayer = new MPlayer(joinRoom.PlayerNm);
            MGameRoom room = mainGameServer.JoinRoom(joinRoom.RoomId, mPlayer);
            LobbyResDto lobbyResDto;
            if (room != null)
            {
                connectedClients.Add(mPlayer.PlayerId, client);
                lobbyResDto = new LobbyResDto()
                {
                    PlayerId = mPlayer.PlayerId,
                    Room = room
                };

                // 같은 게임룸에 있는 사람들에게 새로운 player의 입장을 알린다.
                LobbyStatusResDto lobbyStatusResDto = new LobbyStatusResDto()
                {
                    Leader = room.Leader,
                    Player = mPlayer,
                    Idx = Array.FindIndex(room.Players, p => p != null && p.PlayerId == mPlayer.PlayerId),
                };
                BroadcastToPlayers(room.Players, SerializeData(lobbyStatusResDto, SPacketType.B_P_ENTER));
            }
            else
            {
                lobbyResDto = new LobbyResDto()
                {
                    PlayerId = Guid.Empty,
                    Room = room
                };
            }
            byte[] dataToSend = SerializeData(lobbyResDto, SPacketType.C_ENTERLOBBY);
            client.Send(dataToSend);
        }

        private void Game_StatusChange(byte[] data)
        {
            LobbyStatusReqDto status = JsonSerializer.Deserialize<LobbyStatusReqDto>(data);
            MGameRoom room = mainGameServer.GetRoom(status.RoomId);
            // TODO: Null Error Handling
            if (room == null) return;
            int playerIdx = Array.FindIndex(room.Players, p => p != null && p.PlayerId == status.PlayerId);

            room.Players[playerIdx].IsReady = status.IsReady;
            LobbyStatusResDto broadcastInfo = new LobbyStatusResDto()
            {
                Leader = room.Leader,
                Player = room.Players[playerIdx],
                Idx = playerIdx,
            };
            BroadcastToPlayers(room.Players, SerializeData(broadcastInfo, SPacketType.B_P_CHANGE));
        }

        private void Game_LeaveRoom(byte[] data)
        {
            LobbyStatusReqDto status = JsonSerializer.Deserialize<LobbyStatusReqDto>(data);
            int deletedPlayer = mainGameServer.LeaveRoom(status.RoomId, status.PlayerId);
            // TODO: Null Error Handling
            if (deletedPlayer == -1) return;
            MGameRoom room = mainGameServer.GetRoom(status.RoomId);

            LobbyStatusResDto broadcastInfo = new LobbyStatusResDto()
            {
                Leader = room.Leader,
                Idx = deletedPlayer,
            };
            BroadcastToPlayers(room.Players, SerializeData(broadcastInfo, SPacketType.B_P_LEAVE));
        }

        private void Game_StartRoom(byte[] data)
        {
            Guid roomId = Guid.Parse(Encoding.UTF8.GetString(data));
            MGameRoom room = mainGameServer.GetRoom(roomId);
            room.IsPlaying = true;

            BroadcastToPlayers(room.Players, SerializeData(room, SPacketType.B_GAME_START));
        }

        private void Game_PlayerMove(byte[] data)
        {
            FieldReqDto playerPos = JsonSerializer.Deserialize<FieldReqDto>(data);
            MGameRoom room = mainGameServer.GetRoom(playerPos.Room);
            int playerIdx = Array.FindIndex(room.Players, p => p != null && p.PlayerId == playerPos.Player);

            FieldResDto broadcastInfo = new FieldResDto()
            {
                Idx = playerIdx,
                Pos = playerPos.Pos,
            };
            BroadcastToPlayers(room.Players, SerializeData(broadcastInfo, SPacketType.B_MVE));
        }

        #endregion

        private void BroadcastToPlayers(MPlayer[] players, byte[] data)
        {
            Console.WriteLine("broadcast packet!");
            foreach (MPlayer player in players)
            {
                if (player != null && player.PlayerId != Guid.Empty)
                {
                    try
                    {
                        connectedClients[player.PlayerId].Send(data);
                    } 
                    catch (SocketException noSocket)
                    {
                        Console.WriteLine(noSocket.Message);
                    }
                }
            }
        }

        private static byte[] SerializeData(object dto, SPacketType packetType)
        {
            string dtoSerialized = JsonSerializer.Serialize(dto);
            byte[] dtoToBytes = Encoding.UTF8.GetBytes(dtoSerialized);
            byte[] dataToSend = new byte[dtoToBytes.Length + 1];
            dataToSend[0] = (byte)packetType;
            Array.Copy(dtoToBytes, 0, dataToSend, 1, dtoToBytes.Length);
            return dataToSend;
        }
    }
}
