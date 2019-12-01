using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class PlayerController3D : MonoBehaviourPunCallbacks
{
    private Rigidbody rb;
    private Animator anime;
    [SerializeField]
    private GameObject fireball;
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float offsetFireball = 1f;
    private float offsetFireballY = 1f;
    [SerializeField]
    private float FireballSpeed = 3f;
    public float speedH = 2.0f;
    private float yaw = 0;
    private bool isAttacking = false;
    [SerializeField]
    private GameObject aim;
    [SerializeField]
    private GameObject camera;
    private bool hurt = false;
    private float knockbackTime = 1f;
    private float timer = 0;
    private Vector3 hurtVelcity;
    [SerializeField]
    private float knockbackReduction = 0.1f;
    private float moreKnockback = 1f;
    public TextMesh playerName;
    // Start is called before the first frame update
    void Start()
    {
        anime = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody>();
        if (!photonView.IsMine) {
            aim.SetActive(false);
            camera.SetActive(false);
            playerName.text = photonView.Owner.NickName;
        }
        else {
            OnlineGameManager.camera = camera.transform;
            playerName.gameObject.SetActive(false);
            aim.SetActive(true);
            camera.SetActive(true);
        }
    }
    // Update is called once per frame
    void Update()
    {

        if (!photonView.IsMine) {
            playerName.transform.parent.LookAt(OnlineGameManager.camera);
            return;
        }
        if (!Input.GetKey(KeyCode.K)) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        Attack();

        if (hurt) {
            if (knockbackTime > timer) {
                timer += Time.deltaTime;

                hurtVelcity += -hurtVelcity.normalized * knockbackReduction;

                Debug.Log("After:" +hurtVelcity);
            }
            else {
                timer = 0;
                hurtVelcity = Vector3.zero;
                hurt = false;
            }
        }
        yaw += speedH * Input.GetAxis("Mouse X");
        transform.eulerAngles = new Vector3(0, yaw, 0);
        if (!isAttacking) {
        var Horizontal = Input.GetAxisRaw("Horizontal");
        var Vertical = Input.GetAxisRaw("Vertical");
        Vector3 dirHorizontal = transform.right * Horizontal;
        Vector3 dirVertical = transform.forward * Vertical;
        Vector3 newDir = ((dirHorizontal + dirVertical).normalized * speed) + (Vector3.up * rb.velocity.y);
        rb.velocity = newDir;
        anime.SetInteger("Vertical", (int)Vertical);
        anime.SetInteger("Horizontal", (int)Horizontal);
        }else {
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
        rb.velocity += hurtVelcity;
    }

    void Attack ()
    {
        if (anime.GetCurrentAnimatorStateInfo(0).IsName("Attack")) {
            if (isAttacking) {
                //var fireTemp = Instantiate(fireball, (transform.position + Vector3.up * offsetFireballY + transform.forward.normalized * offsetFireball), Quaternion.identity);
                var fireTemp = PhotonNetwork.Instantiate("fireball", (transform.position + Vector3.up * offsetFireballY + transform.forward.normalized * offsetFireball), Quaternion.identity);
                fireTemp.GetComponent<VelocityFireBall3D>().force = (transform.forward.normalized * FireballSpeed);
                fireTemp.GetPhotonView().RPC("SetOnlineVelocity",RpcTarget.Others, (transform.forward.normalized * FireballSpeed));
                isAttacking = false;
            }
            anime.SetBool("Attack", false);
        }else {
            if (Input.GetButtonDown("Fire1")) {
                anime.SetInteger("Vertical", 0);
                anime.SetInteger("Horizontal", 0);
                anime.SetBool("Attack", true);
                isAttacking = true;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        hurtVelcity = other.GetComponent<VelocityFireBall3D>().force * moreKnockback;
        moreKnockback += 0.1f;
        var photonView = other.GetComponent<VelocityFireBall3D>().photonView;
        photonView.RPC("DestoryFireBall",RpcTarget.All);
        hurt = true;
    }


}
