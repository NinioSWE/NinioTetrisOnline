using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private Sprite[] sprites;
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private Rigidbody2D rigidbody2D;
    [SerializeField]
    private float speed = 1.0f;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

        float angle = lookAtMouse();
        switchImage(fixAngle((angle + 27.5f), 360f));
        Movement();
    }

    void Movement()
    {
        rigidbody2D.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized *speed;
    }


    float lookAtMouse()
    {
        Vector3 mouse = Input.mousePosition;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, transform.position.y));
        Vector3 playerToMouse = mouseWorld - transform.position;
        /*Debug.DrawLine(Vector3.zero, Vector3.up, Color.red, 2.5f);
        Debug.DrawLine(Vector3.zero, playerToMouse, Color.red, 2.5f);*/
        return Vector3.SignedAngle(Vector3.up, new Vector3(playerToMouse.x, playerToMouse.y, 0), Vector3.forward) + 180;

    }

    private void switchImage(float angle)
    {
        //(fixAngle((angle + 27.5f), 360f)
        spriteRenderer.sprite = sprites[(int)(angle/45)];
    }


    private float fixAngle(float x, float limit)
    {
        //negativ
        if (x < 0) {
            x = limit + x; 
        }
        if (x > limit) {
            x = x - limit;
        }

        return x;
    }
}
