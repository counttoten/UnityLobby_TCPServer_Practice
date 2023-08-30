using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasAlertPanelController : MonoBehaviour
{
    public TMP_Text AlertMsg;

    public void OpenAlertPanel(string message)
    {
        gameObject.SetActive(true);
        AlertMsg.text = message;
    }
}
