using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProtocolSet { }

// Packet ===============================================
public enum CPacketType
{
    I_ROOMLIST = 10,
    I_JOINROOM = 11,    // P -> C_ENTERLOBBY, B_P_ENTER
    I_CREATEROOM = 12,  // L -> C_ENTERLOBBY
    L_STATUS = 20,       // P -> B_P_CHANGE
    L_LEAVE = 21,       // A -> B_P_LEAVE
    L_START = 22,       // L -> B_GAME_START 
    F_POSITION = 30,
}

public enum SPacketType
{
    B_ = 0, // B_로 시작하면 BROADCAST TO ALL PLAYERS IN THAT ROOM
    C_ = 1, // C_로 시작되면 특정 CLIENT한테 값 리턴
    C_ROOMLIST = 10,
    C_ENTERLOBBY = 11,

    B_P_ENTER = 20,     // GAMEROOM에 있는 플레이어들한테 또다른 플레이어의 입장을 알림
    B_P_CHANGE = 21,    // GAMEROOM에 있는 플레이어들한테 플레이어의 READY 상태 변화를 알림
    B_P_LEAVE = 22,     // GAMEROOM에 있는 플레이어들한테 플레이어가 나감을 알림

    B_GAME_START = 30,  // GAMEROOM에 있는 플레이어들한테 게임이 시작됨을 알림
}

// DTOs ===============================================
public class LobbyResDto
{
    public Guid PlayerId { get; set; }
    public MGameRoom Room { get; set; }
}

public class CreateRoomReqDto
{
    public string RoomNm { get; set; }
    public string PlayerNm { get; set; }
}

public class JoinRoomReqDto
{
    public Guid RoomId { get; set; }
    public string PlayerNm { get; set; }
}

public class RoomListDto
{
    public Guid RoomId { get; set; }
    public string Name { get; set; }
    public bool IsPlaying { get; set; }
    public string Leader { get; set; }
    public int Mems { get; set; }
}

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
