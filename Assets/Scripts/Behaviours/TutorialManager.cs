using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    public GameObject ballPrefab;
    public GameObject platformPrefab;

    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textLives;
    public TextMeshProUGUI textLevel;
    public TextMeshProUGUI textHighscore;
    public TextMeshProUGUI textTotalScore;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textLevelCompletedTime;

    public GameObject panelPlay;

    GameObject ball;
    GameObject currentLevel;
    GameObject platform;

    private TimeSpan timePlaying;
    private bool timerGoing;

    private float elapsedTime;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
