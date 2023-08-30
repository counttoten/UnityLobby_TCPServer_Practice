using System;
using UnityEngine;

public class FieldManager : MonoBehaviour
{
    public GameObject PlayerObjParent;
    private GameObject[] playerObjs = new GameObject[10];
    private OtherPlayerController[] otherPlayerControllers = new OtherPlayerController[10];

    void Start()
    {
        GameManager.Instance.Field = this;

        for (int i = 0; i < 10; i++)
        {
            playerObjs[i] = PlayerObjParent.transform.GetChild(i).gameObject;
            otherPlayerControllers[i] = playerObjs[i].GetComponent<OtherPlayerController>();
        }

        SetInitState();
    }

    private void SetInitState()
    {
        // �� player�� ���� playerobjs�� child(Player prefab) Ű�� �ѱ�, player controller ��ũ��Ʈ�� �� �� ���� �� ����
        MPlayer[] players = GameManager.Instance.GameRoom.Players;
        int index = 0;
        foreach (var player in players)
        {
            if (player != null && player.PlayerId != Guid.Empty)
            {
                playerObjs[index].SetActive(true);
                if (player.PlayerId != GameManager.Instance.PlayerId) 
                { 
                    playerObjs[index].GetComponent<PlayerController>().enabled = false; 
                }
            }
            else
            {
                playerObjs[index].SetActive(false);
            }
            index++;
        }
        // TODO!!!!
    }

    private void OnDestroy()
    {
        GameManager.Instance.Field = null;
    }
}
