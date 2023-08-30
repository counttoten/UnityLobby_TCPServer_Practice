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

                byte[] dtoSerialized = null;
                SPacketType sendPacketType = SPacketType.C_;
                switch (route)
                {
                    case CPacketType.I_ROOMLIST:
                        sendPacketType = SPacketType.C_ROOMLIST;
                        dtoSerialized = Game_RoomList();
                        break;
                    case CPacketType.I_CREATEROOM:
                        sendPacketType = SPacketType.C_ENTERLOBBY;
                        dtoSerialized = Game_CreateRoom(data, client);
                        break;
                    case CPacketType.I_JOINROOM:
                        sendPacketType = SPacketType.C_ENTERLOBBY;
                        dtoSerialized = Game_JoinRoom(data, client);
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
                }

                if (dtoSerialized != null)
                {
                    Console.WriteLine($"response packet type: {sendPacketType}");
                    byte[] dataToSend = new byte[dtoSerialized.Length + 1];
                    dataToSend[0] = (byte)sendPacketType;
                    Array.Copy(dtoSerialized, 0, dataToSend, 1, dtoSerialized.Length);
                    client.Send(dataToSend);
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
        private byte[] Game_RoomList()
        {
            string dtoSerialized = JsonSerializer.Serialize(mainGameServer.GetRooms());
            byte[] dtoToBytes = Encoding.UTF8.GetBytes(dtoSerialized);
            return dtoToBytes;
        }
        // CPacketType.I_CREATEROOM
        private byte[] Game_CreateRoom(byte[] data, Socket client)
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
            string dtoSerialized = JsonSerializer.Serialize(lobbyResDto);
            byte[] dtoToBytes = Encoding.UTF8.GetBytes(dtoSerialized);
            return dtoToBytes;
        }

        // CPacketType.I_JOINROOM
        private byte[] Game_JoinRoom(byte[] data, Socket client)
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
                    Player = mPlayer,
                    Idx = Array.FindIndex(room.Players, p => p != null && p.PlayerId == mPlayer.PlayerId),
                };
                SendStatusToPlayers(room.Players, lobbyStatusResDto, SPacketType.B_P_ENTER);
            }
            else
            {
                lobbyResDto = new LobbyResDto()
                {
                    PlayerId = Guid.Empty,
                    Room = room
                };
            }
            string dtoSerialized = JsonSerializer.Serialize(lobbyResDto);
            byte[] dtoToBytes = Encoding.UTF8.GetBytes(dtoSerialized);
            return dtoToBytes;
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
                Player = room.Players[playerIdx],
                Idx = playerIdx,
            };

            SendStatusToPlayers(room.Players, broadcastInfo, SPacketType.B_P_CHANGE);
        }

        private void Game_LeaveRoom(byte[] data)
        {
            LobbyStatusReqDto status = JsonSerializer.Deserialize<LobbyStatusReqDto>(data);
            int deletedPlayer = mainGameServer.LeaveRoom(status.RoomId, status.PlayerId);
            // TODO: Null Error Handling
            if (deletedPlayer == -1) return;

            LobbyStatusResDto broadcastInfo = new LobbyStatusResDto()
            {
                Idx = deletedPlayer,
            };
            SendStatusToPlayers(mainGameServer.GetRoom(status.RoomId).Players, broadcastInfo, SPacketType.B_P_LEAVE);
        }

        private void Game_StartRoom(byte[] data)
        {
            Guid roomId = Guid.Parse(Encoding.UTF8.GetString(data));
            MGameRoom room = mainGameServer.GetRoom(roomId);
            room.IsPlaying = true;
            // TODO
            // gameroom 전체 정보 보낼 수 있도록 수정..!
            // 아니다 필드에 가서 game 정보를 받자...! 그게 낫지 않나..? 암튼 고민해보기..!
            SendStatusToPlayers(room.Players, null, SPacketType.B_GAME_START);
        }

        #endregion

        private void SendStatusToPlayers(MPlayer[] players, LobbyStatusResDto dto, SPacketType packetType)
        {
            Console.WriteLine($"broadcast packet type: {packetType}");
            string dtoSerialized = JsonSerializer.Serialize(dto);
            byte[] dtoToBytes = Encoding.UTF8.GetBytes(dtoSerialized);
            byte[] dataToSend = new byte[dtoToBytes.Length + 1];
            dataToSend[0] = (byte)packetType;
            Array.Copy(dtoToBytes, 0, dataToSend, 1, dtoToBytes.Length);
            
            foreach (MPlayer player in players)
            {
                if (player != null && player.PlayerId != Guid.Empty)
                {
                    try
                    {
                        connectedClients[player.PlayerId].Send(dataToSend);
                    } 
                    catch (SocketException noSocket)
                    {
                        Console.WriteLine(noSocket.Message);
                    }
                }
            }
        }
    }
}
