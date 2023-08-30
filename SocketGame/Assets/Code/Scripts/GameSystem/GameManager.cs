using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoSingleton<GameManager>
{
    public NetworkManager Network;
    public LobbyManager Lobby;

    public GameState gameState = GameState.LOADING;

    public Guid PlayerId;
    public MGameRoom GameRoom;

    private bool sceneChangeFlag = false;

    private void Start()
    {
        StartCoroutine(LoadInitScene());
    }

    private void Update()
    {
        if (sceneChangeFlag)
        {
            sceneChangeFlag = false;
            StartCoroutine(LoadAsyncScene());
        }
    }

    IEnumerator LoadInitScene()
    {
        Network = gameObject.AddComponent(typeof(NetworkManager)) as NetworkManager;
        Network.Connect();
        // datamanager player id set.
        DataManager.PlayerName = PlayerPrefs.GetString(DataManager.PlayerPrefKey);
        if (DataManager.PlayerName == "") DataManager.PlayerName = "GUEST_PLAYER";

        if (Application.internetReachability == NetworkReachability.NotReachable) yield break;

        ChangeScene(GameState.IN_INIT);
    }

    private void OnApplicationQuit()
    {
        Network.Close();
    }
    
    public void ChangeScene(GameState _state)
    {
        if (gameState == _state) return;
        gameState = _state;
        sceneChangeFlag = true;
    }
    IEnumerator LoadAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync((int)gameState);
        while (!asyncLoad.isDone) { yield return null; }
    }
}
