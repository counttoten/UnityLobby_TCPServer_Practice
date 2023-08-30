using System;

namespace SocketServer.Models
{
    public class MGameRoom
    {
        public Guid RoomId { get; set; }
        public string Name { get; set; }
        public bool IsPlaying { get; set; }
        public MPlayer[] Players { get; set; }

        public Guid Leader { get; set; }
        private MPlayer leader;

        public MGameRoom(string name)
        {
            RoomId = Guid.NewGuid();
            Name = name;
            IsPlaying = false;
            Players = new MPlayer[10];
            Leader = Guid.Empty;
            leader = null;
        }

        public void AddPlayer(MPlayer _player)
        {
            for (int i = 0; i < 10; i++)
            {
                if (Players[i] == null || Players[i].PlayerId == Guid.Empty)
                {
                    Players[i] = _player;
                    if (leader == null) { leader = _player; Leader = _player.PlayerId; }
                    break;
                }
            }
        }

        public int RemovePlayer(Guid playerId)
        {
            int idx = Array.FindIndex(Players, p => p != null && p.PlayerId == playerId);
            Players[idx] = null;

            if (Leader == playerId)
            {
                FindNewLeader();
            }
            return idx;
        }
        
        public int CountPlayers()
        {
            int cnt = 0;
            foreach (MPlayer player in Players)
            {
                if (player != null && player.PlayerId != Guid.Empty) cnt++;
            }

            return cnt;
        }

        public void FindNewLeader()
        {
            Leader = Guid.Empty;
            leader = null;
            foreach (MPlayer player in Players)
            {
                if (player != null && player.PlayerId != Guid.Empty)
                {
                    Leader = player.PlayerId;
                    leader = player;
                    break;
                }
            }
        }

        public MPlayer GetLeader()
        {
            return leader;
        }
    }
}
