using TMPro;
using UnityEngine;

public class CanvasOptionsController : MonoBehaviour
{
    public GameObject AlertPanel;
    private CanvasAlertPanelController panelController;

    private bool processing;
    private TMP_InputField input;
    
    private void Awake()
    {
        panelController = AlertPanel.GetComponent<CanvasAlertPanelController>();

        processing = false;
        input = gameObject.GetComponentInChildren<TMP_InputField>();
    }

    private void OnEnable()
    {
        input.text = DataManager.PlayerName;
    }

    public void OnSaveButtonClick()
    {
        if (processing) 
        {
            panelController.OpenAlertPanel("Saving username... Please wait!");
            return; 
        }

        processing = true;

        // 1. validation
        string wantedUserName = input.text.Trim();
        if (wantedUserName.Length < 2 )
        {
            panelController.OpenAlertPanel("Username is too SHORT. Make its length longer than 2.");
            processing = false;
            return;
        }
        else if (wantedUserName.Length > 20 )
        {
            panelController.OpenAlertPanel("Username is too LONG. Make its length shorter than 20.");
            processing = false;
            return;
        }

        // 2. save to player pref
        PlayerPrefs.SetString(DataManager.PlayerPrefKey, wantedUserName);
        DataManager.PlayerName = wantedUserName;
        processing = false;
    }
}
