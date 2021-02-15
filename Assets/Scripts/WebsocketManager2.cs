using UnityEngine;
using WebSocketSharp;

public class WebsocketManager2 : MonoBehaviour
{
    public static WebsocketManager2 instance;
    public bool connect = false;

    private void Awake()
    {
        instance = this;
        if (connect)
        {
            Connect();
        }
    }
    private void OnApplicationQuit()
    {
        if (connect)
        {
            CloseConnection();
        }
    }

    #region WebsocketSharp
    WebSocket websocket;

    public void Connect()
    {
        connect = true;
        websocket = new WebSocket(WebsocketManager.instance.url);
        websocket.SslConfiguration.EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls12;
        ListenEvent();
        websocket.ConnectAsync();
    }
    void ListenEvent()
    {
        websocket.OnOpen += (sender, e) =>
        {
            UnityMainThread.instance.AddJob(() =>
            {
                Debug.Log("Connection open!");
                FrontendManager.instance.Prompt("Connected.", true);
                FrontendManager.instance.SubmitUsername();
            });
        };
        websocket.OnError += (sender, e) =>
        {
            UnityMainThread.instance.AddJob(() =>
            {
                Debug.Log("Error!");
                FrontendManager.instance.Prompt("Error: " + e.Message, true);
                Debug.Log(e.Message);
            });
        };
        websocket.OnClose += (sender, e) =>
        {
            UnityMainThread.instance.AddJob(() =>
            {
                Debug.Log("Connection closed!");
                FrontendManager.instance.Prompt("Connection Closed! " + e.Reason, true);
                Debug.Log(e.Code + ": " + e.Reason);
            });
        };
        websocket.OnMessage += (sender, e) =>
        {
            UnityMainThread.instance.AddJob(() =>
            {
                Debug.Log("OnMessage!");
                Debug.Log(e.Data);

                FrontendManager.instance.ReceiveMessageHandler(e.Data);
                FrontendManager.instance.SubmitUsernameResponseHandler(e.Data);
            });
        };
    }
    void CloseConnection()
    {
        websocket.CloseAsync();
    }
    public void Send(object message)
    {
        string jsonMessage = JsonUtility.ToJson(message);
        websocket.SendAsync(jsonMessage, null);
    }
    #endregion
}
