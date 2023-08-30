using System;

namespace SocketServer.Models
{
    public class MPlayer
    {
        public Guid PlayerId { get; set; }
        public string Name { get; set; }
        public bool IsReady { get; set; }

        public MPlayer(string name)
        {
            PlayerId = Guid.NewGuid();
            Name = name;
            IsReady = false;
        }

        public void ChangeState(bool isReady)
        {
            IsReady = isReady;
        }
    }
}
