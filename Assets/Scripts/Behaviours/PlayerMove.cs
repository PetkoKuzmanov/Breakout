using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMove: MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private new Rigidbody2D rigidbody;
    private Vector2 velocity;

    // Start is called before the first frame update
    private void Awake()
    {
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
}
