using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    [SerializeField] private float speed = 2f;

    private new Rigidbody2D rigidbody;
    private Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = Vector2.down * speed;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void FixedUpdate()
    {
        rigidbody.velocity = rigidbody.velocity.normalized * speed;
        velocity = rigidbody.velocity;
    }

    private void OnCollisionEnter(Collision collision)
    {
        print("asd");
        rigidbody.velocity = Vector2.Reflect(velocity, collision.contacts[0].normal);
    }
}
