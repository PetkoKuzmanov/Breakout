using UnityEngine;

public class BrickBehaviour : MonoBehaviour
{
    public int hits;
    public int points;

    private SpriteRenderer spriteRenderer;
    private Color white = new Color(255, 255, 255);
    private Color originalColor;

    private User currentUser;

    // Start is called before the first frame update
    void Start()
    {
        currentUser = GameManager.Instance.getCurrentUser();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        hits--;
        //Brick is destroyed
        if (hits <= 0)
        {
            currentUser.Score += points;
            GameManager.Instance.updateTextScore();

            currentUser.IncrementBricksDestroyed();
            if (currentUser.GetBricksDestroyed() == 20)
            {
                AchievementManager.Instance.NotifyAchievementComplete(3);
            }
            else if (currentUser.GetBricksDestroyed() == 50)
            {
                AchievementManager.Instance.NotifyAchievementComplete(7);
            }

            //If the brick is red
            if (gameObject.CompareTag(GameManager.Instance.bricks[0].tag))
            {
                currentUser.IncrementRedBricksDestroyed();
                if (currentUser.GetRedBricksDestroyed() == 5)
                {
                    AchievementManager.Instance.NotifyAchievementComplete(0);
                }
                else if (currentUser.GetRedBricksDestroyed() == 15)
                {
                    AchievementManager.Instance.NotifyAchievementComplete(4);
                }
            }
            //If the brick is yellow
            else if (gameObject.CompareTag(GameManager.Instance.bricks[1].tag))
            {
                currentUser.IncrementYellowBricksDestroyed();
                if (currentUser.GetYellowBricksDestroyed() == 5)
                {
                    AchievementManager.Instance.NotifyAchievementComplete(1);
                }
                else if (currentUser.GetYellowBricksDestroyed() == 15)
                {
                    AchievementManager.Instance.NotifyAchievementComplete(5);
                }
            }
            //If the brick is blue
            else if (gameObject.CompareTag(GameManager.Instance.bricks[2].tag))
            {
                currentUser.IncrementBlueBricksDestroyed();
                if (currentUser.GetBlueBricksDestroyed() == 5)
                {
                    AchievementManager.Instance.NotifyAchievementComplete(2);
                }
                else if (currentUser.GetBlueBricksDestroyed() == 15)
                {
                    AchievementManager.Instance.NotifyAchievementComplete(6);
                }
            }

            Destroy(gameObject);
        }

        spriteRenderer.color = white;
        Invoke(nameof(RestoreOriginalColor), 0.1f);
    }

    void RestoreOriginalColor()
    {
        spriteRenderer.color = originalColor;
    }
}
