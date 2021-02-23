using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickBehaviour : MonoBehaviour
{

    public int hits = 2;
    public int points = 100;


    SpriteRenderer spriteRenderer;
    Color white = new Color(255, 255, 255);
    Color originalColor;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hits--;
        //Score points
        if (hits <= 0)
        {
            GameManager.Instance.Score += points;
            Destroy(gameObject);
        };

        spriteRenderer.color = white;
        Invoke(nameof(RestoreOriginalColor), 0.1f);
    }

    void RestoreOriginalColor()
    {
        spriteRenderer.color = originalColor;
    }
}
