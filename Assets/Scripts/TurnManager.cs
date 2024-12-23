using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class TurnManager : MonoBehaviourPun
{
    private int currentPlayerIndex = 0;
    private PhotonView photonView;


    public TextMeshProUGUI turnIndicatorText; // UI for turn indication
    public TextMeshProUGUI timerText;        // UI for countdown timer
    public float turnDuration = 15f;         // Turn duration in seconds

    private float timer;
    private bool isTimerRunning = false;


    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartTurn();
        }
    }


    public void StartTurn()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        timer = turnDuration;
        isTimerRunning = true;

        // Notify all players about the turn
        photonView.RPC("NotifyTurn", RpcTarget.All, currentPlayerIndex);
    }



    void Awake()
    {
        photonView = GetComponent<PhotonView>();
        if (photonView == null)
        {
            Debug.LogError("PhotonView component is missing on TurnManager!");
        }
    }


    /*public void StartTurn()
    {
        Debug.Log($"Player {PhotonNetwork.NickName} is attempting to start the turn. IsMasterClient: {PhotonNetwork.IsMasterClient}");
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("Only the MasterClient can start the turn.");
            return;
        }

        if (!PhotonNetwork.InRoom)
        {
            Debug.LogError("Not in a room. Cannot start turn.");
            return;
        }

        if (PhotonNetwork.PlayerList == null || PhotonNetwork.PlayerList.Length == 0)
        {
            Debug.LogError("No players in the room.");
            return;
        }

        if (photonView == null)
        {
            Debug.LogError("PhotonView is not assigned!");
            return;
        }

        currentPlayerIndex = (currentPlayerIndex + 1) % PhotonNetwork.PlayerList.Length;
        Debug.Log($"Turn started for: {PhotonNetwork.PlayerList[currentPlayerIndex].NickName}");
        photonView.RPC("NotifyTurn", RpcTarget.All, PhotonNetwork.PlayerList[currentPlayerIndex].NickName);
    }*/


    /*[PunRPC]
    public void NotifyTurn(string playerName)
    {
        Debug.Log("It's " + playerName + "'s turn!");
    }*/


    void Update()
    {
        if (!isTimerRunning) return;

        /*timer -= Time.deltaTime;
        UpdateTimerUI();

        if (timer <= 0)
        {
            EndTurn();
        }*/

        if (PhotonNetwork.IsMasterClient)
        {
            ManageTimer();
        }
    }

    private void ManageTimer()
    {
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            EndTurn();
        }

        // Broadcast the timer value to all clients
        photonView.RPC("SyncTimer", RpcTarget.All, timer);
    }

    [PunRPC]
    private void SyncTimer(float time)
    {
        timer = time;
        UpdateTimerUI();
    }

    [PunRPC]
    private void UpdateTurn(int newIndex)
    {
        currentPlayerIndex = newIndex;
        Debug.Log($"Now it's Player {currentPlayerIndex}'s turn.");
    }




    [PunRPC]
    void NotifyTurn(int playerIndex)
    {
        currentPlayerIndex = playerIndex;

        // Update the UI
        string playerName = PhotonNetwork.PlayerList[playerIndex].NickName;
        turnIndicatorText.text = $"Turn: {playerName}";
        timerText.text = $"Time Left: {turnDuration:F0}s";
    }

    public void EndTurn()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        isTimerRunning = false;

        // Move to the next player's turn
        currentPlayerIndex = (currentPlayerIndex + 1) % PhotonNetwork.PlayerList.Length;
        StartTurn();
    }

    public void PlayerAction()
    {
        if (!isTimerRunning || PhotonNetwork.LocalPlayer.ActorNumber != currentPlayerIndex + 1) return;

        // Perform the player's action here

        // End the turn early if action is completed
        EndTurn();
    }

    void UpdateTimerUI()
    {
        timerText.text = $"Time Left: {Mathf.Ceil(timer)}s";
    }

    public int GetCurrentPlayerIndex()
    {
        return currentPlayerIndex;
    }

    
}
