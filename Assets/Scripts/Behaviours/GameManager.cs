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

    public enum State { MAIN_MENU, PROFILE_MENU, INIT, PLAY, LEVELCOMPLETED, LOADLEVEL, GAMEOVER, INIT_TUTORIAL, TUTORIAL, INIT_REPLAY, REPLAY }
    private State state;

    public GameObject ballPrefab;
    public GameObject platformPrefab;

    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textLives;
    public TextMeshProUGUI textLevel;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textLevelCompletedTime;

    public TextMeshProUGUI textTotalTime;
    public TextMeshProUGUI textHighestLevel;
    public TextMeshProUGUI textTotalScore;

    public GameObject panelMenu;
    public GameObject panelPlay;
    public GameObject panelLevelCompleted;
    public GameObject panelGameOver;
    public GameObject panelProfileMenu;
    public GameObject panelPause;

    public GameObject[] levels;
    public GameObject tutorialLevel;

    public Button buttonReplay;
    public Button buttonStartGame;

    public TMP_InputField inputFieldNewProfile;
    public TextMeshProUGUI textUsernameTaken;

    private Animator panelMenuAnimator;
    private Animator panelProfileSelectAnimator;

    private List<IObserver> observers = new List<IObserver>();

    private GameObject ball;
    private GameObject currentLevel;
    private GameObject platform;
    private bool isSwithcingState;

    private TimeSpan timePlaying;
    private bool timerGoing;
    private float levelTime;
    private float totalTime;

    private User currentUser;

    private List<float> replay = new List<float>();
    private int replayCount;
    private bool isReplay = false;

    private bool isPaused = false;

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
                if (isReplay)
                {
                    //Replay the game
                    PlayerMove.Instance.SetVelocityFromReplay(replay[replayCount]);
                    replayCount++;
                }
                else
                {
                    //Write to the replay list
                    replay.Add(PlayerMove.Instance.GetDirection());
                }
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
                PauseGameIfEscapePressed();
                break;
            case State.LEVELCOMPLETED:
                break;
            case State.LOADLEVEL:
                break;
            case State.GAMEOVER:
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
                PauseGameIfEscapePressed();
                break;
        }
    }

    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.MAIN_MENU:
                Invoke("MainMenuDelay", 1f);
                break;
            case State.PROFILE_MENU:
                Invoke("ProfileMenuDelay", 1f);
                break;
            case State.INIT:
                Invoke("InitDelay", 1f);
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
                if (currentLevel.name != "Tutorial" && !isReplay)
                {
                    SaveManager.SaveUser(currentUser, replay);
                }
                textTotalTime.text = "Total time: " + TimeSpan.FromSeconds(totalTime).ToString("mm':'ss'.'ff");
                textHighestLevel.text = "Level: " + currentUser.Level;
                textTotalScore.text = "Score: " + currentUser.Score;
                panelGameOver.SetActive(true);
                ChangeState(State.MAIN_MENU, 3f);
                break;
            case State.INIT_TUTORIAL:
                Invoke("InitTutorialDelay", 1f);
                break;
            case State.TUTORIAL:
                Notify("PanelMovePlatform");
                break;
            case State.INIT_REPLAY:
                panelPlay.SetActive(true);
                currentUser = new User("Replay", 2, 0, 0);
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
        }
    }

    void EndState()
    {
        switch (state)
        {
            case State.MAIN_MENU:
                PlayMainMenuAnimation("Menu_End");
                break;
            case State.PROFILE_MENU:
                PlayProfileSelectMenuAnimation("ProfileSelect_End");
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
                Destroy(platform);
                Destroy(currentLevel);
                break;
            case State.INIT_TUTORIAL:
                break;
            case State.TUTORIAL:
                break;
        }
    }

    private IEnumerator HidePanelDelay(GameObject panel, float delay)
    {
        yield return new WaitForSeconds(delay);

        panel.SetActive(false);
    }

    public void ChangeState(State newState, float delay = 0)
    {
        StartCoroutine(ChangeDelay(newState, delay));
    }

    private IEnumerator ChangeDelay(State newState, float delay)
    {
        isSwithcingState = true;
        yield return new WaitForSeconds(delay);
        EndState();
        state = newState;
        BeginState(newState);
        isSwithcingState = false;
    }

    private void ProfileMenuDelay()
    {
        panelProfileMenu.SetActive(true);
        textUsernameTaken.enabled = false;
        PlayProfileSelectMenuAnimation("ProfileSelect_Start");
    }

    private void MainMenuDelay()
    {
        panelMenu.SetActive(true);
        PlayMainMenuAnimation("Menu_Start");
    }

    private void InitDelay()
    {
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
    }

    private void InitTutorialDelay()
    {
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
        levelTime = 0f;

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
            totalTime += Time.deltaTime;
            levelTime += Time.deltaTime;
            timePlaying = TimeSpan.FromSeconds(levelTime);
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

    public void UnpauseGameAndLaunchBallAfterOneSecond()
    {
        ball.BroadcastMessage("LaunchBallAfterOneSecond");
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

    public void StartReplay()
    {
        replay = SaveManager.LoadReplay(ProfileDropdownManager.Instance.GetCurrentUser().Name);
        isReplay = true;
        ChangeState(State.INIT_REPLAY);
    }

    public void InputOnValueChanged(string name)
    {
        //If the username is taken show an error and dont allow the game to be started
        if (SaveManager.DoesUserExist(inputFieldNewProfile.text))
        {
            textUsernameTaken.enabled = true;
            buttonStartGame.interactable = false;
        }
        else
        {
            textUsernameTaken.enabled = false;
            buttonStartGame.interactable = true;
        }
    }

    public void ExitApplication()
    {
        Application.Quit();
    }

    public bool GetIsReplay()
    {
        return isReplay;
    }

    public void PauseGameIfEscapePressed()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            //If the panel is open unpause the game otherwise pause it
            if (panelPause.activeSelf)
            {
                isPaused = false;
                ResumeGame();
            }
            else
            {
                isPaused = true;
                PauseGame();
                panelPause.SetActive(true);
            }
        }
    }

    public void GoToMainMenuAndCloseGame()
    {
        ChangeState(State.MAIN_MENU);
        StopTimer();
        panelPause.SetActive(false);
        panelPlay.SetActive(false);
        Invoke(nameof(DestroyLevelObjects), 0.01f);
    }

    private void DestroyLevelObjects()
    {
        Destroy(platform);
        Destroy(currentLevel);
        Destroy(ball);
    }

    public void ResumeGame()
    {
        isPaused = false;
        panelPause.SetActive(false);
        ball.BroadcastMessage("UnpauseBall");
    }

    public bool GetIsPaused()
    {
        return isPaused;
    }
}
