using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

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

    public GameObject panelMovePlatform;

    public GameObject[] levels;
    public GameObject tutorialLevel;

    public enum State { MENU, INIT, PLAY, LEVELCOMPLETED, LOADLEVEL, GAMEOVER, INIT_TUTORIAL, TUTORIAL }
    State state;

    GameObject ball;
    GameObject currentLevel;
    GameObject platform;
    bool isSwithcingState;

    private TimeSpan timePlaying;
    private bool timerGoing;

    private float elapsedTime;

    private User currentUser;
    public User getCurrentUser()
    {
        return currentUser;
    }

    public void updateTextScore()
    {
        textScore.text = "Score: " + currentUser.Score;
    }

    public void updateTextLives()
    {
        textLives.text = "Lives: " + currentUser.Lives;
    }

    public void updateTextLevel()
    {
        textLevel.text = "Level: " + (currentUser.Level + 1);
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
                if (ball == null)
                {
                    if (currentUser.Lives > 0)
                    {
                        InstantiateBall();
                    }
                    else
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
            case State.INIT_TUTORIAL:
                break;
            case State.TUTORIAL:
                if (ball == null)
                {
                    if (currentUser.Lives > 0)
                    {
                        InstantiateBall();
                    }
                    else
                    {
                        ChangeState(State.GAMEOVER);
                    }
                }
                if (currentLevel != null && currentLevel.transform.childCount == 0 && !isSwithcingState)
                {
                    ChangeState(State.LEVELCOMPLETED);
                }
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
                currentUser = new User("Bob2", 2, 0, 0);
                updateTextScore();
                updateTextLevel();
                updateTextLives();
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
                SoundManager.PlaySound("Level Completed");
                Destroy(ball);
                Destroy(currentLevel);
                currentUser.Level++;
                panelLevelCompleted.SetActive(true);
                ChangeState(State.LOADLEVEL, 2f);
                break;
            case State.LOADLEVEL:
                StopTimer();
                BeginTimer();
                if (currentUser.Level > levels.Length)
                {
                    ChangeState(State.GAMEOVER);
                }
                else
                {
                    currentLevel = Instantiate(levels[currentUser.Level]);
                    ChangeState(State.PLAY);
                }
                break;
            case State.GAMEOVER:
                StopTimer();
                //SaveManager.SaveUser(currentUser);

                if (currentUser.Score > PlayerPrefs.GetInt("highscore"))
                {
                    PlayerPrefs.SetInt("highscore", currentUser.Score);
                }
                textTotalScore.text = "Score: " + currentUser.Score;
                panelGameOver.SetActive(true);
                break;
            case State.INIT_TUTORIAL:
                panelPlay.SetActive(true);
                currentUser = new User("Tutorial", 2, 0, 0);
                updateTextScore();
                updateTextLevel();
                updateTextLives();
                if (currentLevel != null)
                {
                    Destroy(currentLevel);
                }
                platform = Instantiate(platformPrefab);

                StopTimer();
                BeginTimer();
                currentLevel = Instantiate(tutorialLevel);

                ChangeState(State.TUTORIAL);
                break;
            case State.TUTORIAL:
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
            case State.INIT_TUTORIAL:
                break;
            case State.TUTORIAL:
                break;
        }
    }

    public void StartGameClicked()
    {
        ChangeState(State.INIT);
    }

    public void TutorialClicked()
    {
        ChangeState(State.INIT_TUTORIAL);
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
        while (timerGoing)
        {
            elapsedTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(elapsedTime);
            string timeString = "Time: " + timePlaying.ToString("mm':'ss'.'ff");
            textTimer.text = timeString;

            yield return null;
        }
    }
}
