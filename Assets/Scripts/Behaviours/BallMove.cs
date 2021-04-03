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
    private Vector2 velocityBeforePause;

    public GameObject platform;
    public GameObject brickZero;
    public GameObject brickOne;
    public GameObject brickTwo;

    private static int brickHitCounter;

    private List<IObserver> observers = new List<IObserver>();

    private Vector2 emptyVector = new Vector2(0,0);

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

            GameManager.Instance.getCurrentUser().IncrementPlatformHitCounter();
            if (GameManager.Instance.getCurrentUser().GetPlatformHitCounter() == 15)
            {
                AchievementManager.Instance.NotifyAchievementComplete(10);
            }
            else if (GameManager.Instance.getCurrentUser().GetPlatformHitCounter() == 50)
            {
                AchievementManager.Instance.NotifyAchievementComplete(11);
            }
        }
        else
        {
            if (collision.collider.gameObject.CompareTag(brickZero.tag))
            {
                brickHitCounter++;
                SoundManager.PlaySound("Brick 0");
            }
            else if (collision.collider.gameObject.CompareTag(brickOne.tag))
            {
                brickHitCounter++;
                SoundManager.PlaySound("Brick 1");
            }
            else if (collision.collider.gameObject.CompareTag(brickTwo.tag))
            {
                brickHitCounter++;
                SoundManager.PlaySound("Brick 2");
            }

            //Notify the tutorial observer that a brick has been hit for the first time
            if (GameManager.Instance.GetState() == GameManager.State.TUTORIAL)
            {
                NotifyTutorialIfBrickHitForFirstTime();
            }
        }
    }

    public void PauseBall()
    {
        velocityBeforePause = rigidbody.velocity;
        speed = 0;
    }

    public void UnpauseBall()
    {
        if (velocityBeforePause == emptyVector)
        {
            LaunchBallAfterOneSecond();
        }
        else
        {
            rigidbody.velocity = velocityBeforePause;
            velocity = velocityBeforePause;
            speed = 7;
        }
    }

    public void LaunchBallAfterOneSecond()
    {
        speed = 7;
        Invoke(nameof(LaunchBall), 1f);
    }

    public int GetHitCounter()
    {
        return brickHitCounter;
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
        if (brickHitCounter == 1)
        {
            brickHitCounter++;
            Notify("PanelBrick0");
        }
    }

    public void DestroyBall()
    {
        Destroy(this);
    }
}
