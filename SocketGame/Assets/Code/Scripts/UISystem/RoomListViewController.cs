using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomListViewController : MonoBehaviour
{
    public Guid roomId;

    public void SetGameRoom(RoomListDto _room)
    {
        roomId = _room.RoomId;
        transform.GetChild(0).GetComponent<TMP_Text>().text = _room.Name;
        transform.GetChild(1).GetComponent<TMP_Text>().text = _room.Leader;
        transform.GetChild(2).GetComponent<TMP_Text>().text = $"{_room.Mems}/10";
    }

    public void OnViewClicked()
    {

    }
}
