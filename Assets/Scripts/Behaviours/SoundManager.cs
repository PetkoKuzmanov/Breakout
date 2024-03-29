using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; set; }
    static AudioSource audioSource;

    public static AudioClip brickZeroSound, brickOneSound, brickTwoSound, ballDeathSound, levelCompletedSound, buttonClickedSound, gameCompletedSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        brickZeroSound = Resources.Load<AudioClip>("Brick 0 Sound");
        brickOneSound = Resources.Load<AudioClip>("Brick 1 Sound");
        brickTwoSound = Resources.Load<AudioClip>("Brick 2 Sound");
        ballDeathSound = Resources.Load<AudioClip>("Ball Death Sound");
        levelCompletedSound = Resources.Load<AudioClip>("Level Completed Sound");
        buttonClickedSound = Resources.Load<AudioClip>("Button Clicked Sound");
        gameCompletedSound = Resources.Load<AudioClip>("Game Completed Sound");
    }

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public static void PlaySound(string soundName)
    {
        switch (soundName)
        {
            case "Brick 0":
                audioSource.PlayOneShot(brickZeroSound);
                break;
            case "Brick 1":
                audioSource.PlayOneShot(brickOneSound, 0.75f);
                break;
            case "Brick 2":
                audioSource.PlayOneShot(brickTwoSound, 0.9f);
                break;
            case "Ball Death":
                audioSource.PlayOneShot(ballDeathSound);
                break;
            case "Level Completed":
                audioSource.PlayOneShot(levelCompletedSound, 0.9f);
                break;
            case "Button Clicked":
                audioSource.PlayOneShot(buttonClickedSound, 0.1f);
                break;
            case "Game Completed":
                audioSource.PlayOneShot(gameCompletedSound, 0.8f);
                break;
        }
    }
}
