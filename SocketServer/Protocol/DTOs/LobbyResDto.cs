using SocketServer.Models;
using System;

namespace SocketServer.Protocol.DTOs
{
    public class LobbyResDto
    {
        public Guid PlayerId { get; set; }
        public MGameRoom Room { get; set; }
    }
}
