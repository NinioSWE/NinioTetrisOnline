using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBall : MonoBehaviour
{
    [SerializeField]
    private GameObject bulletPrefab;
    [SerializeField]
    private float speed = 1f;
    [SerializeField]
    private float offset = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 mouse = Input.mousePosition;
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(new Vector3(mouse.x, mouse.y, transform.position.y));
        Vector2 playerToMouse = mouseWorld - transform.position;
        if (Input.GetButtonDown("Fire1")) {
            var bullet = Instantiate(bulletPrefab, transform.position + Vector3.Normalize(playerToMouse) * offset, Quaternion.identity);
            bullet.GetComponent<VelocityFireball>().force = Vector3.Normalize(playerToMouse) * speed;
        }
    }
}
