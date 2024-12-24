using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerButtonManager : MonoBehaviourPunCallbacks
{
    public GameObject playerButtonPrefab; // Assign your Button prefab
    public Transform buttonPanel; // Assign the Panel for buttons

    private void Start()
    {
        UpdatePlayerButtons(); // Generate buttons at the start
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdatePlayerButtons(); // Update buttons when a new player joins
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdatePlayerButtons(); // Update buttons when a player leaves
    }

    private void UpdatePlayerButtons()
    {
        // Clear existing buttons
        foreach (Transform child in buttonPanel)
        {
            Destroy(child.gameObject);
        }

        // Create a button for each player
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            GameObject button = Instantiate(playerButtonPrefab, buttonPanel);
            TMPro.TMP_Text buttonText = button.GetComponentInChildren<TMPro.TMP_Text>();
            buttonText.text = player.NickName; // Set player's name on the button
            Debug.Log($"Buttin added to the set player.NickName");
            Debug.Log(player);
            // Add a click listener to the button
            button.GetComponent<Button>().onClick.AddListener(() => OnPlayerButtonClick(player));
        }
    }

    private void OnPlayerButtonClick(Player player)
    {
        Debug.Log($"Clicked on {player.NickName}'s button");
        // Call your private chat start logic here
        FindObjectOfType<ChatManager>().StartPrivateChat(player.NickName);
        FindObjectOfType<ChatManager>().SubscribingToChannel("private_"+player.NickName);
    }
}

