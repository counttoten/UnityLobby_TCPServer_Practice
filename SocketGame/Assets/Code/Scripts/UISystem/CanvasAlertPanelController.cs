using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CanvasAlertPanelController : MonoBehaviour
{
    public TMP_Text AlertTitle;
    public TMP_Text AlertMsg;

    public void OpenAlertPanel(string _title, string _message)
    {
        gameObject.SetActive(true);
        AlertTitle.text = _title;
        AlertMsg.text = _message;
    }
}
