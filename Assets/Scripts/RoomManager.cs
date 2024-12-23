using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Realtime; // For Player class

public class RoomManager : MonoBehaviourPunCallbacks
{
    public TMP_InputField roomNameInput;
    public TMP_InputField nicknameInputField;
    public TMP_Text feedbackText;
    public GameObject playerPrefab; // Assign your player prefab here.
    public GameObject UIBGObject;
    public TMP_Text playerListText;


    public Button gameStartButton;

    private void Start()
    {
        gameStartButton.gameObject.SetActive(false);
}

    public void CreateRoom()
    {
        gameStartButton.gameObject.SetActive(true);
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            feedbackText.text = "Room name cannot be empty!";
            return;
        }

        string roomName = roomNameInput.text;
        Debug.Log("Attempting to create room: " + roomName);
        PhotonNetwork.CreateRoom(roomName);
    }

    public void JoinRoom()
    {
        if (string.IsNullOrEmpty(roomNameInput.text))
        {
            feedbackText.text = "Room name cannot be empty!";
            return;
        }

        string roomName = roomNameInput.text;
        Debug.Log("Attempting to join room: " + roomName);
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        Hide();
        feedbackText.text = "Room created successfully!";
        Debug.Log("Room created: " + PhotonNetwork.CurrentRoom.Name);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        GetNickname();
        Hide();
        feedbackText.text = "Joined room: " + PhotonNetwork.CurrentRoom.Name;
        Debug.Log("Joined room: " + PhotonNetwork.CurrentRoom.Name);

        // Instantiate the player at a random position
        Vector3 spawnPosition = new Vector3(Random.Range(-5f, 5f), 1, Random.Range(-5f, 5f));
        //PhotonNetwork.Instantiate(playerPrefab.name, spawnPosition, Quaternion.identity);
        Debug.Log($"{PhotonNetwork.NickName} joined room: {PhotonNetwork.CurrentRoom.Name}");
        Debug.Log($"IsMasterClient: {PhotonNetwork.IsMasterClient}");
        UpdatePlayerList();
    }


    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        feedbackText.text = "Failed to create room: " + message;
        Debug.LogError("Create Room Failed: " + message);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        feedbackText.text = "Failed to join room: " + message;
        Debug.LogError("Join Room Failed: " + message);
    }


    public void Show()
    {
        UIBGObject.SetActive(true);
    }
    
    public void Hide()
    {
        UIBGObject.SetActive(false);
    }

    private void UpdatePlayerList()
    {
        playerListText.text = "Players in room:\n";
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            playerListText.text += player.NickName + "\n";
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerList();
        feedbackText.text = newPlayer.NickName + " joined the room!";
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerList();
        feedbackText.text = otherPlayer.NickName + " left the room!";
    }

    public void GetNickname()
    {
        if (nicknameInputField != null && !string.IsNullOrEmpty(nicknameInputField.text))
        {
            Debug.Log("Set name via input field! : " + string.IsNullOrEmpty(nicknameInputField.text));
            PhotonNetwork.NickName = nicknameInputField.text;
        }
        else
        {
            Debug.Log("Set name randomly! : " + string.IsNullOrEmpty(nicknameInputField.text));
            PhotonNetwork.NickName = "Player" + Random.Range(1000, 9999); // Default name
        }
    }

}

