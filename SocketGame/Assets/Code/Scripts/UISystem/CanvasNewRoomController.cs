using Newtonsoft.Json;
using System;
using System.Text;
using TMPro;
using UnityEngine;

public class CanvasNewRoomController : MonoBehaviour
{
    public GameObject AlertPanel;
    private CanvasAlertPanelController panelController;
    public GameObject LoadingPanel;

    private bool processing;
    private TMP_InputField input;
    
    private void Awake()
    {
        panelController = AlertPanel.GetComponent<CanvasAlertPanelController>();

        OpenLoadingCanvas(false);
        input = gameObject.GetComponentInChildren<TMP_InputField>();
    }

    private void OpenLoadingCanvas(bool open)
    {
        processing = open;
        LoadingPanel.SetActive(open);
    }

    public void OnCreateButtonClick()
    {
        if (processing) 
        {
            panelController.OpenAlertPanel("PLEASE WAIT", "Waiting for Server to create a new room... Please Wait!");
            return; 
        }

        OpenLoadingCanvas(true);

        // 1. validation
        string wantedRoomName = input.text.Trim();
        if (wantedRoomName.Length < 2 )
        {
            panelController.OpenAlertPanel("ERROR OCCUR", "The name of the room is too SHORT. Make its length longer than 2.");
            OpenLoadingCanvas(false);
            return;
        }
        else if (wantedRoomName.Length > 20 )
        {
            panelController.OpenAlertPanel("ERROR OCCUR", "The name of the room is too LONG. Make its length shorter than 20.");
            OpenLoadingCanvas(false);
            return;
        }

        // 2. send data to server!
        CreateRoomReqDto createRoomReqDto = new CreateRoomReqDto()
        {
            RoomNm = wantedRoomName,
            PlayerNm = DataManager.PlayerName
        };
        string dtoSerialized = JsonConvert.SerializeObject(createRoomReqDto);
        byte[] reqDTO = Encoding.Default.GetBytes(dtoSerialized);
        GameManager.Instance.Network.newRoomController = this;
        GameManager.Instance.Network.SendMessageToServer(CPacketType.I_CREATEROOM, reqDTO);
    }

    public void GotDataFromServer(LobbyResDto lobbyRes)
    {
        try
        {
            if (lobbyRes.PlayerId == null)
            {
                OpenLoadingCanvas(false);
                panelController.OpenAlertPanel("ERROR OCCUR", "Failed to create a new server...");
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
