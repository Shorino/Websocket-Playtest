using UnityEngine;

public class Message
{
    public string action;
    public string data;

    public Message(string action, Message2 data)
    {
        this.action = action;
        this.data = JsonUtility.ToJson(data);
    }

    public Message(string action, string data)
    {
        this.action = action;
        this.data = data;
    }
}

public class Message2
{
    public string data1;
    public string data2;

    public Message2(string data1, string data2)
    {
        this.data1 = data1;
        this.data2 = data2;
    }
}

public class ReceivedMessage
{
    public string action;
    public string message;
    public string sender;
}

public class ReceivedResponse
{
    public string action;
    public bool success;
}