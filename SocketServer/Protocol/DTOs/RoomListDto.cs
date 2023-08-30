using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SocketServer.Protocol.DTOs
{
    public class RoomListDto
    {
        public Guid RoomId { get; set; }
        public string Name { get; set; }
        public bool IsPlaying { get; set; }
        public string Leader { get; set; }
        public int Mems { get; set; }
    }
}
