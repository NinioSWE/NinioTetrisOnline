using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Launcher : MonoBehaviourPunCallbacks
{
    public void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        Connect();
    }
    public void Connect()
    {
        PhotonNetwork.GameVersion = "1.0.1";
        PhotonNetwork.ConnectUsingSettings();

    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Create();
        base.OnJoinRandomFailed(returnCode, message);
    }
    public override void OnConnectedToMaster()
    {
        Join();
        base.OnConnectedToMaster();
    }
    public override void OnJoinedRoom()
    {
        StartGame();
        base.OnJoinedRoom();
    }

    public void Create()
    {
        PhotonNetwork.CreateRoom("");
    }

    public void Join()
    {
        PhotonNetwork.JoinRandomRoom();
    }
    public void StartGame()
    {
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1) {
            PhotonNetwork.LoadLevel(1);
        }
    }
}
