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




    private string targetPlayerName = ""; // The player to chat with


    private string appIdChat = "44a5b792-4407-431f-9bd0-89b96f22b833";


    /*private void Start()
    {
        playerName = PhotonNetwork.NickName; // Use the player's Photon nickname
        ConnectToChat();
    }*/

    private void Start()
    {
        /*playerName = PhotonNetwork.NickName; // Use the player's Photon nickname
        //PhotonNetwork.NickName = playerName;
        chatClient = new ChatClient(this);
        chatClient.Connect(appIdChat, "1.0", new Photon.Chat.AuthenticationValues(playerName));
        Debug.Log($"Attempting to connect to Photon Chat as {playerName}");*/
        playerName = PhotonNetwork.NickName;
        ConnectToChat();
        privateChatUI.SetActive(true); // Hide private chat UI initially
    }

    private void Update()
    {
        if (chatClient != null)
        {
            chatClient.Service();
            //Debug.Log("ChatClient Service Running");
        }
        else
        {
            Debug.Log("ChatClient is NULL");
        }
    }

    private void ConnectToChat()
    {
        Debug.Log("Connect to chat!");
        chatClient = new ChatClient(this);
        chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat, "1.0", new AuthenticationValues(playerName));
    }

    // Method to handle global chat messages
    public void SendGlobalChatMessage()
    {
        if (inputFieldGlobalChat.text != string.Empty)
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

        // Add message to the private chat UI
        AddMessageToPrivateChat($"Me: {inputFieldPrivateChat.text}");
        inputFieldPrivateChat.text = ""; // Clear input field
    }


    [PunRPC]
    public void ReceivePrivateMessage(string senderName, string receiverName, string message, PhotonMessageInfo info)
    {
        // Ensure the message is for the current player
        if (receiverName == PhotonNetwork.NickName)
        {
            AddMessageToChat($"{senderName}: {message}");
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
            }
            else
            {
                AddMessageToPrivateChat($"{senders[i]}: {messages[i]}");
            }
        }
    }

    //Add message to global chat
    private void AddMessageToChat(string message)
    {
        chatDisplay.text += message + "\n";
    }

    // Add message to private chat
    private void AddMessageToPrivateChat(string message)
    {
        privateChatDisplay.text += message + "\n";
    }

    // Handle private message from a specific player
    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        if (isPrivateChatActive)
        {
            AddMessageToPrivateChat($"From {sender}: {message}");
        }
    }


    


    public void OnChatStateChange(ChatState state) { }
    public void DebugReturn(DebugLevel level, string message) { }
    public void OnDisconnected() { }
    public void OnConnected()
    {
        //chatClient.Subscribe("Global"); // Join the Global channel
        SubscribingToChannel("Global");
        AddMessageToChat($"You joined the chat as {playerName}.");
    }


    public void OnUnsubscribed(string[] channels) { }
    //public void OnPrivateMessage(string sender, object message, string channelName) { }
    public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }

    

    public void OnUserSubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        Debug.Log($"Subscribed to channel: {string.Join(", ", channels)}");
        //AddMessageToChat("Subscribed to Global chat.");
    }

    // Click to start a private chat with another player
    public void OnPlayerProfileClick(string playerName)
    {
        StartPrivateChat(playerName); // Start private chat when clicking a player profile
    }

    public void SubscribingToChannel(string str)
    {
        chatClient.Subscribe(str);
    }



}

