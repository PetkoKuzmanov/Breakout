using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove : MonoBehaviour
{
    public static PlayerMove Instance { get; private set; }

    [SerializeField] private float moveSpeed;

    private new Rigidbody2D rigidbody;
    public static Vector2 velocity;

    // Start is called before the first frame update
    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    public void Move(Vector2 direction)
    {
        velocity = rigidbody.velocity;
        velocity.x = direction.x * moveSpeed;
        rigidbody.velocity = velocity;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!GameManager.Instance.GetIsReplay() 
            && !GameManager.Instance.GetIsPaused() 
            && (GameManager.Instance.GetState() == GameManager.State.PLAY || GameManager.Instance.GetState() == GameManager.State.TUTORIAL))
        {
            Move(context.ReadValue<Vector2>());
        }
    }

    public void SetVelocityFromReplay(float replayPosition)
    {
        Vector2 direction = new Vector2(replayPosition, velocity.y);
        Move(direction);
    }

    public float GetDirection()
    {
        return Math.Sign(rigidbody.velocity.x);
    }
}
