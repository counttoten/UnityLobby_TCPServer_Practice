using System;

namespace SocketServer.Protocol
{
    // I_ INIT화면에서 발생하는 PACKET
    // L_ LOBBY화면에서 발생하는 PACKET
    // F_ FIELD화면에서 발생하는 PACKET
    public enum CPacketType
    {
        I_ROOMLIST = 10,
        I_JOINROOM = 11,    // P -> C_ENTERLOBBY, B_P_ENTER
        I_CREATEROOM = 12,  // L -> C_ENTERLOBBY
        L_READY = 20,       // P -> B_P_CHANGE
        L_NOTREADY = 21,    // P -> B_P_CHANGE
        L_LEAVE = 22,       // A -> B_P_LEAVE
        L_START = 23,       // L -> B_GAME_START 
        F_POSITION = 30,
    }
}
