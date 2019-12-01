using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class HoldBlockPreview : MonoBehaviour
{
    public static HoldBlockPreview Instance { get; private set; }
    public PhotonView pv;

    public TetrisPreview tp;
    // Start is called before the first frame update
    void Start()
    {
        if (Instance == null && pv.IsMine) {
            Instance = this;
        }
    }

}
