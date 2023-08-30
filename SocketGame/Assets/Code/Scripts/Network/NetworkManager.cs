using System.Net.Sockets;
using System.Net;
using System;
using System.Threading.Tasks;
using UnityEngine;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using UnityEditor.PackageManager;
using Newtonsoft.Json;

public class NetworkManager : MonoBehaviour
{
    Socket mainSock;
    int m_port = 5000;
    public void Connect()
    {
        mainSock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        IPAddress serverAddr = IPAddress.Parse("127.0.0.1");
        mainSock.Connect(serverAddr, m_port);

        SocketAsyncEventArgs receiveAsync = new SocketAsyncEventArgs();
        receiveAsync.Completed += new EventHandler<SocketAsyncEventArgs>(ReceiveAsync_Completed);
        receiveAsync.SetBuffer(new byte[2048], 0, 2048);
        receiveAsync.UserToken = mainSock;
        mainSock.ReceiveAsync(receiveAsync);
    }
    public void Close()
    {
        if (mainSock != null)
        {
            mainSock.Close();
            mainSock.Dispose();
        }
    }
    public void Send(byte[] msg)
    {
        mainSock.Send(msg);
    }

    private void ReceiveAsync_Completed(object sender, SocketAsyncEventArgs e)
    {
        Socket serverSocket = (Socket)sender;

        if (serverSocket.Connected && e.BytesTransferred > 0)
        {
            int length = e.BytesTransferred;
            byte[] buffer = new byte[length];
            Array.Copy(e.Buffer, buffer, length);

            SPacketType packetType = (SPacketType)buffer[0];
            byte[] data = null;
            if (length > 0)
            {
                data = new byte[length - 1];
                Array.Copy(buffer, 1, data, 0, length - 1);
            }

            HandleReceivedData(packetType, data);

            serverSocket.ReceiveAsync(e);
        }
    }

    struct ReqAndResCallback
    {
        public byte[] data;
        public Action<byte[]> resSetFunc;
    }
    Queue<ReqAndResCallback> networkQueue = new();
    bool isBusy = false;

    public Task<byte[]> SendMessageToServer(CPacketType cPacketType, byte[] msg = null)
    {
        var tcs = new TaskCompletionSource<byte[]>();
        if (msg == null) msg = Array.Empty<byte>();
        byte[] dataToSend = new byte[msg.Length + 1];
        dataToSend[0] = (byte)cPacketType;
        Array.Copy(msg, 0, dataToSend, 1, msg.Length);

        ReqAndResCallback reqAndResCallback = new()
        {
            data = dataToSend,
            resSetFunc = async (result) =>
            {
                /*
                if (result != null && result.MessageType == (int)ResponsePacketType.TOKEN_INVALID && _messageType != RequestPacketType.GET_USER)
                {
                    Debug.Log("Token isn't valid. Resend!");
                    await DataManager.Instance.GetToken();
                    result = await SendMessageToServer(_messageType, _message);
                }
                */
                tcs.TrySetResult(result);
            }
        };

        networkQueue.Enqueue(reqAndResCallback);
        if (!isBusy) StartCoroutine(SendMessageFromQueue());
        return tcs.Task;
    }

    IEnumerator SendMessageFromQueue()
    {
        isBusy = true;
        while (networkQueue.Count > 0)
        {
            ReqAndResCallback rrc = networkQueue.Dequeue();
            yield return StartCoroutine(SendToSocket(rrc));
        }
        isBusy = false;
    }

    IEnumerator SendToSocket(ReqAndResCallback rrc)
    {
        // socket 통신은 req 보내면 res 받아오는 구조가 아니네...? await 할 필요가 없음.. 다시 짜기!
        mainSock.Send(rrc.data);

        rrc.resSetFunc(null);
        yield return null;
    }

    #region ListeningFromServer Objs
    public CanvasNewRoomController newRoomController = null;
    public CanvasRoomListController roomListController = null;
    public LobbyBtn_Join_OnClick joinRoomController = null;

    private void HandleReceivedData(SPacketType packetType, byte[] data)
    {
        string dto = Encoding.UTF8.GetString(data);
        switch (packetType)
        {
            case SPacketType.C_ENTERLOBBY:
                try
                {
                    LobbyResDto lobbyResDto = JsonConvert.DeserializeObject<LobbyResDto>(dto);
                    if (newRoomController != null)
                    {
                        newRoomController.GotDataFromServer(lobbyResDto);
                        newRoomController = null;
                    }
                    else if (joinRoomController != null)
                    {
                        joinRoomController.GotDataFromServer(lobbyResDto);
                        newRoomController = null;
                    }
                }
                catch (Exception e) { Debug.Log(e); }
                break;
            case SPacketType.C_ROOMLIST:
                try
                {
                    List<RoomListDto> roomList = JsonConvert.DeserializeObject<List<RoomListDto>>(dto);
                    roomListController.SetRoomList(roomList);
                    roomListController = null;
                } catch (Exception e) { Debug.Log(e); }
                break;
            case SPacketType.B_P_ENTER:
            case SPacketType.B_P_CHANGE:
            case SPacketType.B_P_LEAVE:
                try
                {
                    LobbyStatusResDto changedPlayer = JsonConvert.DeserializeObject<LobbyStatusResDto>(dto);
                    GameManager.Instance.GameRoom.Players[changedPlayer.Idx] = changedPlayer.Player;
                    GameManager.Instance.Lobby.GameRoomDataChanged();
                }
                catch { }
                break;
        }
    }

#endregion
}
