using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class JoinLobbyButton : MonoBehaviourPunCallbacks
{
    public void joinLobby()
    {
        PhotonNetwork.JoinRoom(gameObject.name);
    }
}
