
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class OnlineGameManager : MonoBehaviourPunCallbacks
{
    public string player_prefab;
    public Transform spawn_point;
    public Transform spawn_point2;
    public Transform spawn_point3;
    public Transform spawn_point4;
    public static Transform camera;


    public static int height = 20;
    public static int width = 10;
    /*public static Transform[,] gridPlayer1 = new Transform[width, height];
    public static Transform[,] gridPlayer2 = new Transform[width, height];*/

    public void Update()
    {
        if (Input.GetKey("escape"))
        {
            Application.Quit();
        }
    }

    private void Start()
    {
        TetrisBlock.stopGame = false;
        Spawn();
    }
    public void Spawn()
    {
        if (PhotonNetwork.PlayerList.Length == 2) {
            
            var player1 = PhotonNetwork.PlayerList[0].ActorNumber;
            var player2 = PhotonNetwork.PlayerList[1].ActorNumber;
            if (PhotonNetwork.LocalPlayer.ActorNumber == player1) {
                PhotonNetwork.Instantiate(player_prefab, spawn_point.position, spawn_point.rotation);
            }
            if (PhotonNetwork.LocalPlayer.ActorNumber == player2) {
                PhotonNetwork.Instantiate(player_prefab, spawn_point2.position, spawn_point2.rotation);
            }
        }
        else {
            var player1 = PhotonNetwork.PlayerList[0].ActorNumber;
            if (PhotonNetwork.LocalPlayer.ActorNumber == player1) {
                PhotonNetwork.Instantiate(player_prefab, spawn_point.position, spawn_point.rotation);
            }
        }
        InvokeRepeating("speedUp",10,10);

    }


    public void speedUp()
    {
        TetrisBlock.fallTime -= 0.04f;
    }



}
