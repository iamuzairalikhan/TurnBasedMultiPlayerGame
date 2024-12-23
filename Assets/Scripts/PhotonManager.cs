using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using TMPro;

public class PhotonManager : MonoBehaviourPunCallbacks
{
    //public TMP_InputField nicknameInputField;

    void Start()
    {
        /*if (nicknameInputField != null && !string.IsNullOrEmpty(nicknameInputField.text))
        {
            Debug.Log("Set name via input field! : " + string.IsNullOrEmpty(nicknameInputField.text)); 
            PhotonNetwork.NickName = nicknameInputField.text;
        }
        else
        {
            Debug.Log("Set name randomly! : " + string.IsNullOrEmpty(nicknameInputField.text)); 
            PhotonNetwork.NickName = "Player" + Random.Range(1000, 9999); // Default name
        }*/

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon!");
        PhotonNetwork.JoinLobby(); // Join the default lobby
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined the lobby!");
    }
}

