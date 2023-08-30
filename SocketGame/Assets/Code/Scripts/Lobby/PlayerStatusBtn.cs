using Newtonsoft.Json;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

enum StatusBtnType
{
    P_WAIT,
    P_READY,
    L_DISABLED,
    L_START,
}

public class PlayerStatusBtn : MonoBehaviour
{
    public Button button;
    StatusBtnType btnStatus;

    public async void OnClick()
    {
        switch (btnStatus)
        {
            case StatusBtnType.P_WAIT:
                btnStatus = StatusBtnType.P_READY;
                break;
            case StatusBtnType.P_READY:
                btnStatus = StatusBtnType.P_WAIT;
                break;
            case StatusBtnType.L_START:
                break;
            case StatusBtnType.L_DISABLED:
                return;
        }

        string dtoSerialized = string.Empty;
        CPacketType packetType = CPacketType.L_STATUS;
        switch (btnStatus) // 눌린 후의 상태
        {
            case StatusBtnType.P_READY:
            case StatusBtnType.P_WAIT:
                LobbyStatusReqDto dto = new LobbyStatusReqDto()
                {
                    RoomId = GameManager.Instance.GameRoom.RoomId,
                    PlayerId = GameManager.Instance.PlayerId,
                    IsReady = btnStatus == StatusBtnType.P_READY,
                };
                dtoSerialized = JsonConvert.SerializeObject(dto);
                break;
            case StatusBtnType.L_START:
                dtoSerialized = GameManager.Instance.GameRoom.RoomId.ToString();
                packetType = CPacketType.L_START;
                break;
        }
        byte[] reqDTO = Encoding.Default.GetBytes(dtoSerialized);
        await GameManager.Instance.Network.SendMessageToServer(packetType, reqDTO);
    }

    public void SetButtonStatus(bool IsLead, bool IsReady)
    {
        if (IsLead)
        {
            btnStatus = IsReady ? StatusBtnType.L_START : StatusBtnType.L_DISABLED;
        }
        else
        {
            btnStatus = IsReady ? StatusBtnType.P_READY : StatusBtnType.P_WAIT;
        }

        switch (btnStatus)
        {
            case StatusBtnType.P_WAIT:
            case StatusBtnType.P_READY:
            case StatusBtnType.L_START:
                button.enabled = true;
                break;
            case StatusBtnType.L_DISABLED:
                button.enabled = false;
                break;
        }
    }
}
