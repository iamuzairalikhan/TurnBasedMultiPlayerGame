using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;

public class PlayerActions : MonoBehaviourPun
{

    public TextMeshProUGUI waitForTurnText;

    public void PerformAction()
    {
        Debug.Log("Performing action!");
        if (!IsPlayerTurn())
        {
            Debug.Log("It's not your turn!");
            StartCoroutine(TextToShowForAWhile("Wait for your turn please!"));
            return;
        }

        // Example action: Move a character
        Debug.Log("Player performed an action!");
        StartCoroutine(TextToShowForAWhile("Player performed action!"));

        // Notify the TurnManager to end the turn
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        turnManager.EndTurn();
    }

    private bool IsPlayerTurn()
    {
        TurnManager turnManager = FindObjectOfType<TurnManager>();
        return PhotonNetwork.LocalPlayer.ActorNumber == turnManager.GetCurrentPlayerIndex() + 1;
    }

    public IEnumerator TextToShowForAWhile(string msg)
    {
        waitForTurnText.text = msg;
        yield return new WaitForSeconds(5f);
        waitForTurnText.text = "";
    }
}

