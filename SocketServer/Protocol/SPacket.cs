namespace SocketServer.Protocol
{
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
}
