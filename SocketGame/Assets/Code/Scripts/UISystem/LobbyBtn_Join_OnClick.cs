using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class LobbyBtn_Join_OnClick : MonoBehaviour, IPointerEnterHandler
{
    EventSystem es;
    private Guid joinRoomId = Guid.Empty;

    public GameObject AlertPanel;
    private CanvasAlertPanelController panelController;
    public GameObject LoadingPanel;

    private void Start()
    {
        panelController = AlertPanel.GetComponent<CanvasAlertPanelController>();
    }

    private void OnEnable()
    {
        es = EventSystem.current;
        joinRoomId = Guid.Empty;
    }

    private bool waitEndFlag = false;
    private void Update()
    {
        if (waitEndFlag)
        {
            waitEndFlag = false;
            LoadingPanel.SetActive(false);
            panelController.OpenAlertPanel("Failed to join the room...");
        }
    }


    public void OnJoinButtonClicked()
    {
        if (joinRoomId == Guid.Empty)
        {
            panelController.OpenAlertPanel("No Selected Room!");
        }
        else
        {
            Debug.Log(joinRoomId);
            JoinRoomReqDto joinRoomReqDto = new JoinRoomReqDto()
            {
                RoomId = joinRoomId,
                PlayerNm = DataManager.PlayerName
            };
            LoadingPanel.SetActive(true);
            string dtoSerialized = JsonConvert.SerializeObject(joinRoomReqDto);
            byte[] reqDTO = Encoding.Default.GetBytes(dtoSerialized);
            GameManager.Instance.Network.joinRoomController = this;
            GameManager.Instance.Network.SendMessageToServer(CPacketType.I_JOINROOM, reqDTO);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        RoomListViewController roomView;
        GameObject selectedObj = es.currentSelectedGameObject;
        if (selectedObj != null && selectedObj.TryGetComponent(out roomView))
        {
            joinRoomId = roomView.roomId;
        }
        else
        {
            joinRoomId = Guid.Empty;
        }
    }

    public void GotDataFromServer(LobbyResDto lobbyRes)
    {
        try
        {
            if (lobbyRes.PlayerId == null || lobbyRes.PlayerId == Guid.Empty)
            {
                waitEndFlag = true;
                return;
            }

            // 다음 로비 씬 열도록 주문 넣기..
            GameManager.Instance.PlayerId = lobbyRes.PlayerId;
            GameManager.Instance.GameRoom = lobbyRes.Room;
            GameManager.Instance.ChangeScene(GameState.IN_LOBBY);
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }
    }
}
