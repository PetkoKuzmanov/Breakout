using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum State { MAIN_MENU, PROFILE_MENU, INIT, PLAY, LEVELCOMPLETED, LOADLEVEL, GAMEOVER, INIT_TUTORIAL, TUTORIAL }
    private State state;

    public GameObject ballPrefab;
    public GameObject platformPrefab;

    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textLives;
    public TextMeshProUGUI textLevel;
    public TextMeshProUGUI textTotalScore;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textLevelCompletedTime;

    public GameObject panelMenu;
    public GameObject panelPlay;
    public GameObject panelLevelCompleted;
    public GameObject panelGameOver;
    public GameObject panelProfileMenu;

    public GameObject[] levels;
    public GameObject tutorialLevel;

    public Button buttonReplay;

    public TMP_InputField inputFieldNewProfile;

    private Animator panelMenuAnimator;
    private Animator panelProfileSelectAnimator;

    private List<IObserver> observers = new List<IObserver>();

    private GameObject ball;
    private GameObject currentLevel;
    private GameObject platform;
    private bool isSwithcingState;

    private TimeSpan timePlaying;
    private bool timerGoing;
    private float elapsedTime;

    private User currentUser;

    private List<ReplayPosition> replay = new List<ReplayPosition>();

    // Start is called before the first frame update
    void Start()
    {
        observers.Add(TutorialObserver.Instance);
        panelMenuAnimator = panelMenu.GetComponent<Animator>();
        panelProfileSelectAnimator = panelProfileMenu.GetComponent<Animator>();
        Instance = this;
        ChangeState(State.MAIN_MENU);
    }

    public void FixedUpdate()
    {
        switch (state)
        {
            case State.PLAY:
                replay.Add(new ReplayPosition(platform.transform.position));
                Debug.Log(platform.transform.position);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case State.MAIN_MENU:
                break;
            case State.PROFILE_MENU:
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
                ChangeState(State.MAIN_MENU, 3f);
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
                        if (currentUser.Lives == 1)
                        {
                            Notify("PanelBallDeath");
                        }
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

    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.MAIN_MENU:
                panelMenu.SetActive(true);
                PlayMainMenuAnimation("Menu_Start");
                break;
            case State.PROFILE_MENU:
                panelProfileMenu.SetActive(true);
                PlayProfileSelectMenuAnimation("ProfileSelect_Start");
                //ProfileMenuSelected();
                break;
            case State.INIT:
                panelPlay.SetActive(true);
                currentUser = new User(inputFieldNewProfile.text, 2, 0, 0);
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
                currentUser.Time = textTimer.text.Substring(6);
                if (currentLevel.name != "Tutorial")
                {
                    SaveManager.SaveUser(currentUser, replay);
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
                Notify("PanelMovePlatform");
                break;
        }
    }

    void EndState()
    {
        switch (state)
        {
            case State.MAIN_MENU:
                PlayMainMenuAnimation("Menu_End");
                //panelMenu.SetActive(false);
                break;
            case State.PROFILE_MENU:
                PlayProfileSelectMenuAnimation("ProfileSelect_End");
                //panelProfileMenu.SetActive(false);
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

    public void StartTimer()
    {
        timerGoing = true;
        StartCoroutine(UpdateTimer());
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

    public void PauseGame()
    {
        ball.BroadcastMessage("PauseBall");
        StopTimer();
    }

    public void UnpauseGame()
    {
        ball.BroadcastMessage("UnpauseBall");
        StartTimer();
    }

    public void ProfileSelectClicked()
    {
        ChangeState(State.PROFILE_MENU);
    }

    public void BackToMainMenuClicked()
    {
        ChangeState(State.MAIN_MENU);
    }

    private void PlayMainMenuAnimation(string name)
    {
        if (panelMenuAnimator != null)
        {
            panelMenuAnimator.Play(name);
        }
    }

    private void PlayProfileSelectMenuAnimation(string name)
    {
        if (panelProfileSelectAnimator != null)
        {
            panelProfileSelectAnimator.Play(name);
        }
    }

    private void Notify(string notificationName)
    {
        foreach (IObserver observer in observers)
        {
            observer.OnNotify(notificationName);
        }
    }

    public State GetState()
    {
        return state;
    }
}
