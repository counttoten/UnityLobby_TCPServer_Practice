using System;

namespace SocketServer.Protocol.DTOs
{
    public class JoinRoomReqDto
    {
        public Guid RoomId { get; set; }
        public string PlayerNm { get; set; }
    }
}
