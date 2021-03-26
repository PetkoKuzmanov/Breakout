using System;
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

    private int hitCounter;

    private List<IObserver> observers = new List<IObserver>();

    // Start is called before the first frame update
    void Start()
    {
        observers.Add(TutorialObserver.Instance);
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
            GameManager.Instance.getCurrentUser().Lives--;
            GameManager.Instance.updateTextLives();
            Destroy(gameObject);
            SoundManager.PlaySound("Ball Death");
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 platformVelocity = PlayerMove.velocity;
        float x = platformVelocity.x;


        //Fixes the ball moving too vertically or horizontally
        if (Math.Abs(velocity.y) > 5 * Math.Abs(velocity.x) && velocity.x != 0)
        {
            velocity.y = 5 * Math.Abs(velocity.x);

        }
        else if (Math.Abs(velocity.x) > 5 * Math.Abs(velocity.y) && velocity.y != 0)
        {
            velocity.x = 5 * Math.Abs(velocity.y);
        }

        rigidbody.velocity = Vector2.Reflect(velocity, collision.contacts[0].normal);

        //Check what the object being hit is
        if (collision.collider.gameObject.CompareTag(platform.tag))
        {
            rigidbody.velocity = new Vector2(x + rigidbody.velocity.x, rigidbody.velocity.y);
        }
        else
        {
            //Notify the tutorial observer that a brick has been hit for the first time
            if (GameManager.Instance.GetState() == GameManager.State.TUTORIAL)
            {
                NotifyTutorialIfBrickHitForFirstTime();
            }

            hitCounter++;
            if (collision.collider.gameObject.CompareTag(brickZero.tag))
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

    }

    public void PauseBall()
    {
        speed = 0;
    }

    public void UnpauseBall()
    {
        speed = 7;
        Invoke(nameof(LaunchBall), 1f);
    }

    public int GetHitCounter()
    {
        return hitCounter;
    }

    private void Notify(string notificationName)
    {
        foreach (IObserver observer in observers)
        {
            observer.OnNotify(notificationName);
        }
    }

    private void NotifyTutorialIfBrickHitForFirstTime()
    {
        if (hitCounter == 0)
        {
            Notify("PanelBrick0");
        }
    }
}
