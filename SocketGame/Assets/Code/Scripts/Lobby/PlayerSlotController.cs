using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlotController : MonoBehaviour
{
    public GameObject PlayerObj;
    private Player playerUI;
    public MPlayer LobbyPlayer;

    private void Start()
    {
        PlayerObj = transform.GetChild(0).gameObject;
        // 움직이는 물체가 아니므로 필드에 쓰이는 스크립트 끄기
        playerUI = PlayerObj.GetComponent<Player>();
        PlayerObj.GetComponent<PlayerController>().enabled = false;
        PlayerObj.SetActive(false);
    }

    // Set Player
    public void PlayerEnter(MPlayer _lobbyPlayer, bool _isLead)
    {
        PlayerObj.SetActive(true);
        LobbyPlayer = _lobbyPlayer;

        playerUI.SetUI(_lobbyPlayer.Name, _lobbyPlayer.IsReady, _isLead);
    }
    
    // Set Player null
    public void PlayerLeave()
    {
        // name 등 전부 null 로 설정
        PlayerObj.SetActive(false);
        LobbyPlayer = null;
    }

}
