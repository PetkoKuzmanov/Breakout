using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMove : MonoBehaviour
{
    [SerializeField] private float speed;

    private new Rigidbody2D rigidbody;
    private Vector2 velocity;
    private new Renderer renderer;

    public GameObject platform;
    public GameObject brickZero;
    public GameObject brickOne;
    public GameObject brickTwo;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        renderer = GetComponent<Renderer>();
        Invoke(nameof(LaunchBall), 1f);
    }

    void LaunchBall()
    {
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

        if (!renderer.isVisible)
        {
            //GameManager.Instance.Lives--;
            GameManager.Instance.getCurrentUser().Lives--;
            GameManager.Instance.updateTextLives();
            Destroy(gameObject);
            SoundManager.PlaySound("Ball Death");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 currentSpeed = PlayerMove.velocity;
        float x = currentSpeed.x;

        rigidbody.velocity = Vector2.Reflect(velocity, collision.contacts[0].normal);

        if (collision.collider.gameObject.CompareTag(platform.tag))
        {
            rigidbody.velocity = new Vector2(x + rigidbody.velocity.x, rigidbody.velocity.y);
        }
        else if (collision.collider.gameObject.CompareTag(brickZero.tag))
        {
            SoundManager.PlaySound("Brick 0");
        }
        else if (collision.collider.gameObject.CompareTag(brickOne.tag))
        {
            SoundManager.PlaySound("Brick 1");
        }
        else if (collision.collider.gameObject.CompareTag(brickTwo.tag))
        {
            SoundManager.PlaySound("Brick 2");
        }

    }

    public void PauseBall()
    {
        speed = 0;
    }

    public void UnpauseBall()
    {
        speed = 7;
    }
}
