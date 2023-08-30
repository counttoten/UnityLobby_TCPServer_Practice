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

        // �����κ��� ���� ���� �ʱ�ȭ �� Init Scene���� ���ư���
        GameManager.Instance.GameRoom = null;
        GameManager.Instance.PlayerId = Guid.Empty;
        GameManager.Instance.ChangeScene(GameState.IN_INIT);
    }
}
