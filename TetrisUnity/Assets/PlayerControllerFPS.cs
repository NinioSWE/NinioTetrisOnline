using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerControllerFPS : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Camera camera;
    [SerializeField]
    private GameObject playerModel;
    private Rigidbody rb;
    [SerializeField]
    private Animator anime;
    [SerializeField]
    private float speed = 1f;
    public float speedH = 2.0f;
    private float yaw = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine) {
            camera.gameObject.SetActive(false);
        }
        else {
            playerModel.transform.position = new Vector3(playerModel.transform.position.x, playerModel.transform.position.y-100, playerModel.transform.position.z);
            camera.gameObject.SetActive(true);
        }
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine) {
            return;
        }
        var Horizontal = Input.GetAxisRaw("Horizontal");
        var Vertical = Input.GetAxisRaw("Vertical");
        Vector3 dirHorizontal = new Vector3(camera.transform.right.x, 0, camera.transform.right.z) * Horizontal;
        Vector3 dirVertical = new Vector3(camera.transform.forward.x, 0, camera.transform.forward.z) * Vertical;
        Vector3 newDir = ((dirHorizontal + dirVertical).normalized * speed) + (Vector3.up * rb.velocity.y);
        rb.velocity = newDir;
        anime.SetInteger("Vertical", (int)Vertical);
        anime.SetInteger("Horizontal", (int)Horizontal);
    }
}
