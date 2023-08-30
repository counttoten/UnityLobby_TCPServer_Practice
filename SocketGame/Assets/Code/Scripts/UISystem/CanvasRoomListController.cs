using System.Collections.Generic;
using UnityEngine;

public class CanvasRoomListController : MonoBehaviour
{
    public GameObject RoomListPrefab;
    public GameObject ScrollviewContent;

    public GameObject LoadingPanel;
    public GameObject NoRoomMessage;

    private List<RoomListDto> roomList;
    private bool roomListUpdateFlag = false;

    public void OnEnable()
    {
        RequestRoomListFromServer();
    }

    private void Update()
    {
        if (roomListUpdateFlag)
        {
            LoadingPanel.SetActive(false);
            roomListUpdateFlag = false;
            if (roomList.Count == 0)
            {
                NoRoomMessage.SetActive(true);
                return;
            }
            NoRoomMessage.SetActive(false);
            int index = 0;
            foreach (RoomListDto room in roomList)
            {
                index++;
                GameObject obj;
                if (ScrollviewContent.transform.childCount < index)
                {
                    obj = Instantiate(RoomListPrefab, ScrollviewContent.transform);
                }
                else
                {
                    obj = ScrollviewContent.transform.GetChild(index - 1).gameObject;
                }

                RoomListViewController roomListView = obj.GetComponentInChildren<RoomListViewController>();
                roomListView.SetGameRoom(room);
            }
        }
    }

    public void SetRoomList(List<RoomListDto> gameRooms)
    {
        roomList = gameRooms;
        roomListUpdateFlag = true;
    }

    public void RequestRoomListFromServer()
    {
        GameManager.Instance.Network.roomListController = this;
        GameManager.Instance.Network.SendMessageToServer(CPacketType.I_ROOMLIST);
        LoadingPanel.SetActive(true);
    }

}
