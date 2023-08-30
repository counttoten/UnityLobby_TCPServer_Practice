using SocketServer.Models;
using SocketServer.Protocol.DTOs;
using System;
using System.Collections.Generic;

namespace SocketServer
{
    internal class GameServer
    {
        private Dictionary<Guid, MGameRoom> _rooms = new Dictionary<Guid, MGameRoom>();

        public MGameRoom CreateRoom(string roomName)
        {
            MGameRoom newRoom = new MGameRoom(roomName);
            _rooms.Add(newRoom.RoomId, newRoom);
            return newRoom;
        }

        public MGameRoom JoinRoom(Guid roomId, MPlayer player)
        {
            if (_rooms.ContainsKey(roomId))
            {
                MGameRoom room = _rooms[roomId];
                if (room.CountPlayers() >= 10) return null;

                room.AddPlayer(player);
                return room;
            }
            else
            {
                Console.WriteLine("Room not found");
                return null;
            }
        }

        public void LeaveRoom(Guid roomId, Guid playerId)
        {
            if (_rooms.ContainsKey(roomId))
            {
                MGameRoom room = _rooms[roomId];
                room.RemovePlayer(playerId);
            }
            else
            {
                Console.WriteLine("Room not found");
            }
        }

        public void RefreshRoomState(Guid roomId)
        {
            // 한 플레이어가 상태 변화시 이 코드도 같이 실행 해 같은 방에 있는 사람들에게 누가 ready/wait 했다는 것을 알린다.
        }

        public void StartGame(Guid roomId)
        {
            // Start button Clicked from lobby
        }

        public List<RoomListDto> GetRooms()
        {
            List<RoomListDto> res = new List<RoomListDto>();
            foreach (var room in _rooms.Values)
            {
                RoomListDto roomListDto = new RoomListDto()
                {
                    RoomId = room.RoomId,
                    Name = room.Name,
                    IsPlaying = room.IsPlaying,
                    Leader = room.GetLeader().Name,
                    Mems = room.CountPlayers(),
                };
                res.Add(roomListDto);
            }
            return res;
        }
    }
}
