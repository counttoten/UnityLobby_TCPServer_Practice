using UnityEngine;

enum StatusBtnType
{
    P_WAIT,
    P_READY,
    L_DISABLED,
    L_START,
}

public class PlayerStatusBtn : MonoBehaviour
{
    StatusBtnType btnStatus;

    public void OnClick()
    {
        switch (btnStatus)
        {
            case StatusBtnType.P_WAIT:
                btnStatus = StatusBtnType.P_READY;
                break;
            case StatusBtnType.P_READY:
                btnStatus = StatusBtnType.P_WAIT; break;
            case StatusBtnType.L_DISABLED:
                break;
            case StatusBtnType.L_START:
                //start!
                break;
        }
    }
}
