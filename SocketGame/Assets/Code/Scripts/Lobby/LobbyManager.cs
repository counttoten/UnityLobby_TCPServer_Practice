using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    MGameRoom room;
    MPlayer player;

    public GameObject PlayerStands;
    PlayerSlotController[] playerSlots;

    private Camera cam;

    public GameObject CanvasObj;
    private TMP_Text playersCntText;
    private TMP_Text descriptionText;
    private GameObject playerStatusBtnObj;
    private PlayerStatusBtn playerStatusBtn;
    private TMP_Text statusBtnText;

    bool initFlag = true;
    bool getDataFromServerFlag = false;
    int myRotationDegree = 0;

    private static readonly int MEMBERS = 10;
    private static readonly int ROTATION_DEGREE = 360 / MEMBERS;

    void Start()
    {
        GameManager.Instance.Lobby = this;

        playerSlots = PlayerStands.GetComponentsInChildren<PlayerSlotController>();
        
        cam = Camera.main;

        playersCntText = CanvasObj.transform.GetChild(2).GetComponent<TMP_Text>();
        descriptionText = CanvasObj.transform.GetChild(3).GetComponent<TMP_Text>();

        playerStatusBtnObj = CanvasObj.transform.GetChild(4).gameObject;
        playerStatusBtn = playerStatusBtnObj.GetComponent<PlayerStatusBtn>();
        statusBtnText = playerStatusBtnObj.GetComponentInChildren<TMP_Text>();

        getDataFromServerFlag = true;
    }

    void Update()
    {
        if (getDataFromServerFlag)
        {
            getDataFromServerFlag = false;
            GetDataFromServer();
        }
    }

    private void OnDestroy()
    {
        GameManager.Instance.Lobby = null;
    }

    static Vector3 rightTurn = new Vector3 (0,ROTATION_DEGREE,0);
    static Vector3 leftTurn = -1 * rightTurn;
    public void RotateCamera(bool isLeft)
    {
        cam.transform.Rotate(isLeft ? leftTurn : rightTurn);
        playerStatusBtnObj.SetActive((int)cam.transform.localRotation.eulerAngles.y == myRotationDegree);
    }

    public void GameRoomDataChanged()
    {
        getDataFromServerFlag = true;
    }

    private void GetDataFromServer()
    {
        try
        {
            room = GameManager.Instance.GameRoom;
            int lobbyPlayerCnt = 0;
            for (int i = 0; i < MEMBERS; i++)
            {
                try
                {
                    if (room.Players[i].PlayerId != Guid.Empty)
                    {
                        lobbyPlayerCnt++;
                        playerSlots[i].PlayerEnter(room.Players[i], room.Players[i].PlayerId == room.Leader);

                        if (room.Players[i].PlayerId == GameManager.Instance.PlayerId)
                        {
                            player = room.Players[i];
                        }
                    }
                    else
                    {
                        playerSlots[i].PlayerLeave();
                    }
                }
                catch
                {
                    playerSlots[i].PlayerLeave();
                }
            }
            // set Canvas Text
            // check if leader and change buttons and playerin(1) text
            // if leader
            if (GameManager.Instance.PlayerId == room.Leader)
            {
                bool canStart = room.CheckCanStart();
                descriptionText.text = "START THE GAME WHEN ALL PLAYERS ARE READY";
                statusBtnText.text = canStart ? "START" : "WAIT";
                playerStatusBtn.SetButtonStatus(true, canStart);
            }
            else
            {
                descriptionText.text = "CLICK WAIT WHEN YOU ARE READY";
                statusBtnText.text = player.IsReady ? "WAIT" : "READY";
                playerStatusBtn.SetButtonStatus(false, player.IsReady);
            }
            playersCntText.text = $"PLAYERS: {lobbyPlayerCnt}/{MEMBERS}";

            if (initFlag)
            {
                initFlag = false;

                int i = Array.FindIndex(room.Players, p => p.PlayerId == GameManager.Instance.PlayerId) * ROTATION_DEGREE;
                cam.transform.Rotate(0, i, 0);
                myRotationDegree = (int)cam.transform.localRotation.eulerAngles.y;
                Debug.Log(myRotationDegree);
            }
        }
        catch 
        {
            if (initFlag) GetDataFromServer();
        }
    }
}
