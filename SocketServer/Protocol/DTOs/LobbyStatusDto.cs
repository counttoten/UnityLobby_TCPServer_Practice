using SocketServer.Models;
using System;

namespace SocketServer.Protocol.DTOs
{
    // L_READY, L_NOTREADY 시 이걸 보냄

    // L_LEAVE 시 이걸 보냄 (ISREADY는 무시)
    public class LobbyStatusReqDto
    {
        public Guid RoomId { get; set; }
        public Guid PlayerId { get; set; }
        public bool IsReady { get; set; }
    }

    public class LobbyStatusResDto
    {
        public MPlayer Player { get; set; }
        public int Idx { get; set; }
    }
}
