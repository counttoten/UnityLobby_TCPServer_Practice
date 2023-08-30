using UnityEngine;

public enum LobbyBtnType
{
    INIT_START,
    INIT_OPTIONS,
    ROOM_LIST_CREATE,
    ROOM_LIST_JOIN,
    NEW_ROOM_CREATE,
    ALERT_PANEL_CLOSE,
    ALL_GOBACK,
}

public class LobbyBtn_OnClick : MonoBehaviour
{
    public LobbyBtnType type;
    public void OnClick()
    {
        switch (type)
        {
            case LobbyBtnType.INIT_START:
                OpenCanvas(2);
                break;
            case LobbyBtnType.INIT_OPTIONS:
                OpenCanvas(1);
                break;
            case LobbyBtnType.ROOM_LIST_CREATE:
                OpenCanvas(1);
                break;
            case LobbyBtnType.ROOM_LIST_JOIN:
                break;
            case LobbyBtnType.NEW_ROOM_CREATE: 
                break;
            case LobbyBtnType.ALERT_PANEL_CLOSE:
                gameObject.transform.parent.parent.parent.gameObject.SetActive(false);
                break;
            case LobbyBtnType.ALL_GOBACK:
                if (gameObject.name.EndsWith("2")) OpenCanvas(-2);
                else OpenCanvas(-1);
                break;
        }
    }

    private void OpenCanvas(int sibling)
    {
        Transform canvasParent = gameObject.transform.parent;
        canvasParent.parent.GetChild(canvasParent.GetSiblingIndex() + sibling).gameObject.SetActive(true);
        canvasParent.gameObject.SetActive(false);
    }
}
