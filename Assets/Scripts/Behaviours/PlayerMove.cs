using System.Collections;
using System.Collections.Generic;
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
        Instance = this;
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
        Move(context.ReadValue<Vector2>());
    }

    public Vector2 getVelocity()
    {
        return velocity;
    }

    public void SetVelocityFromReplay(float replayPosition)
    {
        Vector2 direction = new Vector2(replayPosition, velocity.y);

        //Debug.Log(replayPosition);
        //rigidbody.MovePosition(direction);
        //rigidbody.transform.position.Set(replayPosition, rigidbody.transform.position.y, rigidbody.transform.position.z);
        Move(direction);
    }

    public float GetDirection()
    {
        if (rigidbody.velocity.x == 0)
        {
            return 0;
        }
        else if (rigidbody.velocity.x < 0)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}
