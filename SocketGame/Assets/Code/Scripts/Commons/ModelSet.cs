using System.Collections.Generic;
using System;

public class ModelSet { }

public class MGameRoom
{
    public Guid RoomId { get; set; }
    public string Name { get; set; }
    public bool IsPlaying { get; set; }
    public MPlayer[] Players { get; set; }
    public Guid Leader { get; set; }

    // 4�� �̻��� player && ��� ready���� ������ �� �ִ�!
    public bool CheckCanStart()
    {
        int cnt = 0;
        foreach (var player in Players)
        {
            if (player.PlayerId != Guid.Empty)
            {
                cnt++;
                if (!player.IsReady) return false;
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