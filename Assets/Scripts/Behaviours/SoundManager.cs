using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance { get; private set; }
    static AudioSource audioSource;

    public static AudioClip brickZeroSound, brickOneSound, brickTwoSound, ballDeathSound, LevelCompletedSound;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        brickZeroSound = Resources.Load<AudioClip>("Brick 0 Sound");
        brickOneSound = Resources.Load<AudioClip>("Brick 1 Sound");
        brickTwoSound = Resources.Load<AudioClip>("Brick 2 Sound");
        ballDeathSound = Resources.Load<AudioClip>("Ball Death Sound");
        LevelCompletedSound = Resources.Load<AudioClip>("Level Completed Sound");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Awake()
    {
        if (instance)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
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
                audioSource.PlayOneShot(LevelCompletedSound, 0.9f);
                break;
        }
    }
}
