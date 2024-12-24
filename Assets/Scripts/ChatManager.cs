using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Chat;
using Photon.Pun;
using ExitGames.Client.Photon;
using System.Linq;

public class ChatManager : MonoBehaviour, IChatClientListener
{
    private ChatClient chatClient;
    public string playerName;

    public TMPro.TextMeshProUGUI chatDisplay; // Assign in the Inspector
    public TMPro.TMP_InputField inputFieldGlobalChat; // Assign in the Inspector

    public Button privateSendTextButton;
    public GameObject privateChatUI; // Private chat UI
    public TMPro.TMP_InputField inputFieldPrivateChat; // Assign in the Inspector
    public TMPro.TextMeshProUGUI privateChatDisplay; // Private chat message display

    private string privateChatRecipient = ""; // Store the recipient's name
    private bool isPrivateChatActive = false; // Flag to check if private chat is active

    public GameObject globalChatNotification; // Notification for global chat (e.g., icon or counter)
    public GameObject privateChatNotification; // Notification for private chat (e.g., icon or counter)

    private string targetPlayerName = ""; // The player to chat with

    private string appIdChat = "44a5b792-4407-431f-9bd0-89b96f22b833";

    private void Start()
    {
        playerName = PhotonNetwork.NickName;
        ConnectToChat();
        //privateChatUI.SetActive(false); // Hide private chat UI initially
        globalChatNotification.SetActive(false); // Hide global chat notification
        privateChatNotification.SetActive(false); // Hide private chat notification
    }

    private void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
        }
        else
        {
            Debug.Log("ChatClient is NULL");
        }
    }

    private void ConnectToChat()
    {
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(playerName));
    }

    // Method to handle global chat messages
    public void SendGlobalChatMessage()
    {
        if (!string.IsNullOrEmpty(inputFieldGlobalChat.text))
        {
            chatClient.PublishMessage("Global", inputFieldGlobalChat.text); // Send to Global chat
            inputFieldGlobalChat.text = string.Empty;
        }
    }

    // Method to start private chat with a specific player
    public void StartPrivateChat(string recipient)
    {
        privateChatRecipient = recipient;
        targetPlayerName = recipient;
        privateChatUI.SetActive(true); // Show private chat UI
        privateChatDisplay.text = ""; // Clear the previous chat
        AddMessageToPrivateChat($"You are now chatting with {recipient}");
        privateChatNotification.SetActive(false); // Clear private chat notifications when active
    }

    // Method to send private messages to the recipient
    public void SendPrivateMessage()
    {
        if (string.IsNullOrEmpty(inputFieldPrivateChat.text))
            return;

        if (string.IsNullOrEmpty(targetPlayerName))
        {
            Debug.LogError("Target player not selected.");
            return;
        }

        PhotonView photonView = PhotonView.Get(this);
        if (photonView == null)
        {
            Debug.LogError("PhotonView is missing on ChatManager!");
            return;
        }

        Photon.Realtime.Player targetPlayer = PhotonNetwork.CurrentRoom.Players.Values
            .FirstOrDefault(p => p.NickName == targetPlayerName);

        if (targetPlayer == null)
        {
            Debug.LogError($"Target player {targetPlayerName} not found in the room.");
            return;
        }

        photonView.RPC("ReceivePrivateMessage", targetPlayer, PhotonNetwork.NickName, targetPlayerName, inputFieldPrivateChat.text);

        AddMessageToPrivateChat($"Me: {inputFieldPrivateChat.text}");
        inputFieldPrivateChat.text = ""; // Clear input field
    }

    [PunRPC]
    public void ReceivePrivateMessage(string senderName, string receiverName, string message, PhotonMessageInfo info)
    {
        if (receiverName == PhotonNetwork.NickName)
        {
            AddMessageToPrivateChat($"{senderName}: {message}");
            Debug.Log("notification is not working properly!");
            if (!privateChatUI.activeSelf)
            {
                privateChatNotification.SetActive(true); // Show notification for private chat
            }
            else
            {
                privateChatUI.SetActive(true);
                privateChatNotification.SetActive(true);
            }
            ShowNotificationForSometime(privateChatNotification);
        }
    }

    // Handle incoming messages (both private and global)
    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        for (int i = 0; i < senders.Length; i++)
        {
            if (channelName == "Global")
            {
                AddMessageToChat($"{senders[i]}: {messages[i]}");

                if (!chatDisplay.gameObject.activeSelf)
                {
                    globalChatNotification.SetActive(true); // Show notification for global chat
                    chatDisplay.gameObject.SetActive(true);
                }
                else
                {
                    globalChatNotification.SetActive(true);
                    chatDisplay.gameObject.SetActive(true);
                }
                ShowNotificationForSometime(globalChatNotification);
            }
            else
            {
                AddMessageToPrivateChat($"{senders[i]}: {messages[i]}");
            }
        }
    }

    public IEnumerator ShowNotificationForSometime(GameObject obj)
    {
        yield return new WaitForSeconds(5f);
        obj.SetActive(false);
    }

    private void AddMessageToChat(string message)
    {
        chatDisplay.text += message + "\n";
    }

    private void AddMessageToPrivateChat(string message)
    {
        privateChatDisplay.text += message + "\n";
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        AddMessageToPrivateChat($"From {sender}: {message}");
    }

    public void OnChatStateChange(ChatState state) { }
    public void DebugReturn(DebugLevel level, string message) { }
    public void OnDisconnected() { }
    public void OnConnected()
    {
        SubscribingToChannel("Global");
        AddMessageToChat($"You joined the chat as {playerName}.");
    }

    public void OnUnsubscribed(string[] channels) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }

    public void OnUserSubscribed(string channel, string user) { }
    public void OnUserUnsubscribed(string channel, string user) { }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log($"Subscribed to channel: {string.Join(", ", channels)}");
    }

    public void OnPlayerProfileClick(string playerName)
    {
        StartPrivateChat(playerName); // Start private chat when clicking a player profile
    }

    public void SubscribingToChannel(string str)
    {
        chatClient.Subscribe(str);
    }
}
