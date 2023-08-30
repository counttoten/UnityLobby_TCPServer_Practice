using Newtonsoft.Json;
using System;
using System.Text;
using UnityEngine;

public class LobbyBtn_Leave_OnClick : MonoBehaviour
{
    public async void OnLeaveButtonClicked()
    {
        LobbyStatusReqDto lobbyStatusReqDto = new LobbyStatusReqDto()
        {
            RoomId = GameManager.Instance.GameRoom.RoomId,
            PlayerId = GameManager.Instance.PlayerId
        };
        string dtoSerialized = JsonConvert.SerializeObject(lobbyStatusReqDto);
        byte[] reqDTO = Encoding.Default.GetBytes(dtoSerialized);
        await GameManager.Instance.Network.SendMessageToServer(CPacketType.L_LEAVE, reqDTO);

        // 서버로부터 얻은 정보 초기화 후 Init Scene으로 돌아가기
        GameManager.Instance.GameRoom = null;
        GameManager.Instance.PlayerId = Guid.Empty;
        GameManager.Instance.ChangeScene(GameState.IN_INIT);
    }
}
