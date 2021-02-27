using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject ballPrefab;
    public GameObject platformPrefab;

    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textLives;
    public TextMeshProUGUI textLevel;
    public TextMeshProUGUI textHighscore;
    public TextMeshProUGUI textTotalScore;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textLevelCompletedTime;

    public GameObject panelMenu;
    public GameObject panelPlay;
    public GameObject panelLevelCompleted;
    public GameObject panelGameOver;

    public GameObject[] levels;

    public static GameManager Instance { get; private set; }

    public enum State { MENU, INIT, PLAY, LEVELCOMPLETED, LOADLEVEL, GAMEOVER}
    State state;

    GameObject ball;
    GameObject currentLevel;
    GameObject platform;
    bool isSwithcingState;

    private TimeSpan timePlaying;
    private bool timerGoing;

    private float elapsedTime;

    private int score;
    public int Score
    {
        get { return score; }
        set { score = value;
            textScore.text = "Score: " + score;
        }
    }

    private int level;
    public int Level
    {
        get { return level; }
        set { level = value;
            textLevel.text = "Level: " + (level + 1);
        }
    }

    private int lives;
    public int Lives
    {
        get { return lives; }
        set { lives = value;
            textLives.text = "Lives: " + lives;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        ChangeState(State.MENU);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.MENU:
                break;
            case State.INIT:
                break;
            case State.PLAY:
                if (ball == null) {
                    if (Lives > 0)
                    {
                        //Invoke("InstantiateBall", 0.5f);
                        //ball = Instantiate(ballPrefab);
                        InstantiateBall();
                    } else
                    {
                        ChangeState(State.GAMEOVER);
                    }
                }
                if (currentLevel != null && currentLevel.transform.childCount == 0 && !isSwithcingState)
                {
                    ChangeState(State.LEVELCOMPLETED);
                }
                break;
            case State.LEVELCOMPLETED:
                break;
            case State.LOADLEVEL:
                break;
            case State.GAMEOVER:
                //if (Input.anyKeyDown)
                //{
                Destroy(platform);
                ChangeState(State.MENU, 3f);
                //}
                break;
        }
    }

    public void ChangeState(State newState, float delay = 0)
    {
        StartCoroutine(ChangeDelay(newState, delay));
    }

    IEnumerator ChangeDelay(State newState, float delay)
    {
        isSwithcingState = true;
        yield return new WaitForSeconds(delay);
        EndState();
        state = newState;
        BeginState(newState);
        isSwithcingState = false;
    }

    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.MENU:
                textHighscore.text = "Highscore: " + PlayerPrefs.GetInt("highscore");
                panelMenu.SetActive(true);
                break;
            case State.INIT:
                panelPlay.SetActive(true);
                Score = 0;
                Level = 0;
                Lives = 2;
                if (currentLevel != null)
                {
                    Destroy(currentLevel);
                }
                platform = Instantiate(platformPrefab);
                ChangeState(State.LOADLEVEL);
                break;
            case State.PLAY:
                break;
            case State.LEVELCOMPLETED:
                StopTimer();
                textLevelCompletedTime.text = "Level time: " + timePlaying.ToString("mm':'ss'.'ff");
                Destroy(ball);
                Destroy(currentLevel);
                Level++;
                panelLevelCompleted.SetActive(true);
                ChangeState(State.LOADLEVEL, 2f);
                break;
            case State.LOADLEVEL:
                StopTimer();
                BeginTimer();
                if (Level > levels.Length)
                {
                    ChangeState(State.GAMEOVER);
                } else
                {
                    currentLevel = Instantiate(levels[Level]);
                    ChangeState(State.PLAY);
                }
                break;
            case State.GAMEOVER:
                StopTimer();
                if (Score > PlayerPrefs.GetInt("highscore"))
                {
                    PlayerPrefs.SetInt("highscore", Score);
                }
                textTotalScore.text = "Score: " + Score;
                panelGameOver.SetActive(true);
                break;
        }
    }

    void EndState()
    {
        switch (state)
        {
            case State.MENU:
                panelMenu.SetActive(false);
                break;
            case State.INIT:
                break;
            case State.PLAY:
                break;
            case State.LEVELCOMPLETED:
                panelLevelCompleted.SetActive(false);
                break;
            case State.LOADLEVEL:
                break;
            case State.GAMEOVER:
                panelPlay.SetActive(false);
                panelGameOver.SetActive(false);
                break;
        }
    }

    public void StartGameClicked()
    {
        ChangeState(State.INIT);
    }

    private void InstantiateBall()
    {
        ball = Instantiate(ballPrefab);
    }

    private void BeginTimer()
    {
        textTimer.text = "Time: 00:00.00";
        timerGoing = true;
        elapsedTime = 0f;

        StartCoroutine(UpdateTimer());
    }

    public void StopTimer()
    {
        timerGoing = false;
    }

    private IEnumerator UpdateTimer()
    {
        while(timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timeString = "Time: " + timePlaying.ToString("mm':'ss'.'ff");
            textTimer.text = timeString;

            yield return null;
        }
    }
}
