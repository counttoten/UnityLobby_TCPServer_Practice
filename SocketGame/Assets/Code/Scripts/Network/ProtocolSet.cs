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
    B_ = 0, // B_�� �����ϸ� BROADCAST TO ALL PLAYERS IN THAT ROOM
    C_ = 1, // C_�� ���۵Ǹ� Ư�� CLIENT���� �� ����
    C_ROOMLIST = 10,
    C_ENTERLOBBY = 11,

    B_P_ENTER = 20,     // GAMEROOM�� �ִ� �÷��̾������ �Ǵٸ� �÷��̾��� ������ �˸�
    B_P_CHANGE = 21,    // GAMEROOM�� �ִ� �÷��̾������ �÷��̾��� READY ���� ��ȭ�� �˸�
    B_P_LEAVE = 22,     // GAMEROOM�� �ִ� �÷��̾������ �÷��̾ ������ �˸�

    B_GAME_START = 30,  // GAMEROOM�� �ִ� �÷��̾������ ������ ���۵��� �˸�
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
