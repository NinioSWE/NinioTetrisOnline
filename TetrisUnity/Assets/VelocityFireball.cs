using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VelocityFireball : MonoBehaviour
{
    public Vector2 force;

    // Update is called once per frame
    void Update()
    {
        GetComponent<Rigidbody2D>().velocity = force;
    }
}
