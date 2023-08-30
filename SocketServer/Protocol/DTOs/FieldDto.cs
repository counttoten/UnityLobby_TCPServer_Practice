using System;
using System.Numerics;

namespace SocketServer.Protocol.DTOs
{
    public class FieldReqDto
    {
        public Guid Room { get; set; }
        public Guid Player { get; set; }
        public Vector3 Pos { get; set; }
    }

    public class FieldResDto
    {
        public int Idx { get; set; }
        public Vector3 Pos { get; set; }
    }
}
