using System;

public class ModelSet { }

public class MGameRoom
{
    public Guid RoomId { get; set; }
    public string Name { get; set; }
    public bool IsPlaying { get; set; }
    public MPlayer[] Players { get; set; }
    public Guid Leader { get; set; }

    // 4명 이상의 player && 모두 ready여야 시작할 수 있다!
    public bool CheckCanStart()
    {
        int cnt = 0;
        foreach (var player in Players)
        {
            if (player != null && player.PlayerId != Guid.Empty)
            {
                cnt++;
                // leader는 준비 안해도 됨.
                if (!player.IsReady && player.PlayerId != Leader) return false;
            }
        }
        return cnt >= 4;
    }
}

public class MPlayer
{
    public Guid PlayerId { get; set; }
    public string Name { get; set; }
    public bool IsReady { get; set; }
}