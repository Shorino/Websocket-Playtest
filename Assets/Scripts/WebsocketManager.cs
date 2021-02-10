using UnityEngine;
using NativeWebSocket;

public class WebsocketManager : MonoBehaviour
{
    public static WebsocketManager instance;
    string url = "wss://hvsy18pboi.execute-api.ap-southeast-1.amazonaws.com/staging";
    public bool connect = false;

    private void Awake()
    {
        instance = this;
        if (connect)
        {
            Connect();
        }
    }
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        if (connect)
        {
            websocket.DispatchMessageQueue();
        }
#endif
    }
    private void OnApplicationQuit()
    {
        if (connect)
        {
            CloseConnection();
        }
    }

    #region NativeWebsocket
    WebSocket websocket;

    public async void Connect()
    {
        connect = true;
        websocket = new WebSocket(url);
        ListenEvent();
        await websocket.Connect();
    }
    void ListenEvent()
    {
        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
            FrontendManager.instance.SubmitUsername();
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error!");
            Debug.Log(e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
            Debug.Log(e);
        };

        websocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            var data = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log(data);

            FrontendManager.instance.ReceiveMessageHandler(data);
            FrontendManager.instance.SubmitUsernameResponseHandler(data);
        };
    }
    async void CloseConnection()
    {
        await websocket.Close();
    }
    public async void Send(object message)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.SendText(JsonUtility.ToJson(message));
        }
    }
    #endregion

    #region Socket IO
    /*Socket socket = null;

    public void Connect()
    {

    }
    void ListenEvent()
    {

    }
    void CloseConnection()
    {

    }
    public void Send(object message)
    {

    }*/
    #endregion
}