using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityFireBall3D : MonoBehaviourPunCallbacks
{
    public Vector3 force;
    private float timer = 0;
    public float timeLimit = 6;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody>().velocity = force;
        if (photonView.IsMine) {
            if (timeLimit > timer) {
                timer += Time.deltaTime;
            }else {
                PhotonNetwork.Destroy(photonView);
            }
        }
        //Debug.Log(GetComponent<Rigidbody>().velocity);
    }

    [PunRPC]
    void DestoryFireBall()
    {
        if (photonView.IsMine) {
            PhotonNetwork.Destroy(photonView);
        }
    }


    [PunRPC]
    void SetOnlineVelocity(Vector3 force)
    {
        this.force = force;
    }

}
