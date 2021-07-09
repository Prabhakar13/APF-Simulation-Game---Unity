using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stones : MonoBehaviour
{
    private float speed;
    private Rigidbody2D myRigidbody;
    private Rigidbody2D rb;
    private Vector3 change;
    // Start is called before the first frame update
    void Start()
    {
        speed = 0;
        myRigidbody = GetComponent<Rigidbody2D>();
        rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
    }

    // Update is called once per frame
    void Update()
    {
        change = Vector3.zero;

        change.z = (float)0;
        change.x = (float)0;
        change.y = (float)0;
        MoveCharacter();
    }

    void MoveCharacter()
    {
        myRigidbody.MovePosition(
            transform.position + change * speed * Time.deltaTime
            );
        rb.velocity = new Vector2(0.0f, 0.0f);
        Quaternion deltaRotation = Quaternion.Euler(change * Time.deltaTime);
        myRigidbody.MoveRotation(
            transform.rotation * deltaRotation
            );
    }
}
