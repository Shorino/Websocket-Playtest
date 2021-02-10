using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class FrontendManager : MonoBehaviour
{
    public static FrontendManager instance;

    private void Awake()
    {
        instance = this;
        Prompt();
        InitializePromptOkayButton();
        GoToPage1();
        InitializeConnectButton();
        ClearTextMessage();
        InitializeSendButton();
    }
    private void Update()
    {
        UpdateSendInput();
    }

    #region Prompt
    [Header("Prompt")]
    public GameObject prompt;
    public TextMeshProUGUI promptText;
    public Button promptOkayButton;

    void Prompt(string message, bool showButton)
    {
        prompt.SetActive(true);
        promptText.text = message;
        promptOkayButton.gameObject.SetActive(showButton);
    }
    void Prompt()
    {
        prompt.SetActive(false);
    }
    void InitializePromptOkayButton()
    {
        promptOkayButton.onClick.AddListener(() =>
        {
            Prompt();
        });
    }
    #endregion

    #region Scenes
    [Header("Scenes")]
    public int currentScene = 0;
    public GameObject[] scenes;

    void GoToScene(int scene)
    {
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i].SetActive(false);
        }
        scenes[scene].SetActive(true);
    }
    void GoToPage1()
    {
        currentScene = 0;
        GoToScene(currentScene);
    }
    void GoToPage2()
    {
        currentScene = 1;
        GoToScene(currentScene);
    }
    #endregion

    #region Page 1
    [Header("Page 1")]
    public TMP_InputField nameInputField;
    public Button connectButton;

    void InitializeConnectButton()
    {
        connectButton.onClick.AddListener(() =>
        {
            Prompt("Connecting... Please Wait", false);
            WebsocketManager.instance.Connect();
        });
    }
    public void SubmitUsername()
    {
        string username = nameInputField.text;
        Message data = new Message("submitUsername", username);
        WebsocketManager.instance.Send(data);
    }
    public void SubmitUsernameResponseHandler(string response)
    {
        try
        {
            ReceivedResponse responseObj = JsonUtility.FromJson<ReceivedResponse>(response);
            Debug.Log(response);
            if(responseObj.action == "submitUsername")
            {
                if(responseObj.success)
                {
                    Prompt("Connected.", true);
                    GoToPage2();
                }
                else
                {
                    Prompt("Failed to Connect, please try again later", true);
                }
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
    #endregion

    #region Page 2
    [Header("Page 2")]
    public TMP_InputField messageInputField;
    public Button sendButton;
    public TextMeshProUGUI[] textMessages;

    void ClearTextMessage()
    {
        foreach (var item in textMessages)
        {
            item.text = "";
        }
    }
    void InitializeSendButton()
    {
        sendButton.onClick.AddListener(SendWrittenMessage);
    }
    void UpdateSendInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendWrittenMessage();
        }
    }
    void SendWrittenMessage()
    {
        if (currentScene == 1)
        {
            string message = messageInputField.text;
            Message data = new Message("message", message);
            WebsocketManager.instance.Send(data);
        }
    }
    public void ReceiveMessageHandler(string data)
    {
        try
        {
            ReceivedMessage receivedMessage = JsonUtility.FromJson<ReceivedMessage>(data);
            if(receivedMessage.action == "replyToMessage")
            {
                messageInputField.text = "";
                PushToTextMessages(receivedMessage);
            }
        }
        catch(Exception e)
        {
            Debug.Log(e);
        }
    }
    void PushToTextMessages(ReceivedMessage receivedMessage)
    {
        for (int i = textMessages.Length - 1; i >= 0; i--)
        {
            if(i == 0)
            {
                textMessages[i].text = receivedMessage.sender + ": " + receivedMessage.message;
            }
            else
            {
                textMessages[i].text = textMessages[i - 1].text;
            }
        }
    }
    #endregion
}
