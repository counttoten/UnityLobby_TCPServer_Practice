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
        // �����̴� ��ü�� �ƴϹǷ� �ʵ忡 ���̴� ��ũ��Ʈ ����
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
        // name �� ���� null �� ����
        PlayerObj.SetActive(false);
        LobbyPlayer = null;
    }

}
