using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum State
    {
        MAIN_MENU,
        PROFILE_MENU,
        INIT,
        PLAY,
        LEVELCOMPLETED,
        LOADLEVEL,
        GAMEOVER,
        GAME_COMPLETED,
        INIT_TUTORIAL,
        TUTORIAL,
        INIT_REPLAY,
        REPLAY
    }
    private State state;

    public GameObject ballPrefab;
    public GameObject platformPrefab;

    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textLives;
    public TextMeshProUGUI textLevel;
    public TextMeshProUGUI textTimer;
    public TextMeshProUGUI textLevelCompletedTime;

    public TextMeshProUGUI textAllLevelsCompleted;
    public TextMeshProUGUI textGameOverTitle;
    public TextMeshProUGUI textTotalTime;
    public TextMeshProUGUI textHighestLevel;
    public TextMeshProUGUI textTotalScore;

    public GameObject panelMenu;
    public GameObject panelPlay;
    public GameObject panelLevelCompleted;
    public GameObject panelGameOver;
    public GameObject panelProfileMenu;
    public GameObject panelPause;
    public GameObject canvasUserAchievements;
    public GameObject panelUserStats;

    public GameObject[] levels;
    public GameObject tutorialLevel;

    public Button buttonStartGame;
    public Button buttonAchievements;
    public Button buttonReplay;

    public TMP_InputField inputFieldNewProfile;
    public TextMeshProUGUI textUsernameError;

    private Animator panelMenuAnimator;
    private Animator panelProfileSelectAnimator;

    private List<IObserver> observers = new List<IObserver>();

    private GameObject ball;
    private GameObject currentLevel;
    private GameObject platform;
    private bool isSwitchingState;

    private TimeSpan timePlaying;
    private bool timerGoing;
    private float levelTime;
    private float totalTime;
    private string formattedTime;

    private User currentUser;

    private List<float> replay = new List<float>();
    private int replayCount;
    private bool isReplay = false;

    private bool isPaused = false;

    public List<GameObject> bricks = new List<GameObject>();

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
                        ProfileDropdownManager.Instance.FillDropdownWithUsers();
                        ChangeState(State.GAMEOVER);
                    }
                }
                if (currentLevel != null && currentLevel.transform.childCount == 0 && !isSwitchingState)
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
            case State.GAME_COMPLETED:
                break;
            case State.INIT_TUTORIAL:
                break;
            case State.TUTORIAL:
                if (ball == null)
                {
                    if (currentUser.Lives > 0)
                    {
                        InstantiateBall();
                        if (currentUser.Lives == 998)
                        {
                            Notify("PanelBallDeath");
                        }
                    }
                    else
                    {
                        ChangeState(State.GAMEOVER);
                    }
                }
                if (currentLevel != null && currentLevel.transform.childCount == 0 && !isSwitchingState)
                {
                    ChangeState(State.GAMEOVER);
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

                Destroy(ball);
                panelLevelCompleted.SetActive(true);
                if (currentUser.Level == 5)
                {
                    ChangeState(State.GAME_COMPLETED);
                }
                else
                {
                    SoundManager.PlaySound("Level Completed");
                    Destroy(currentLevel);
                    currentUser.Level++;
                    ChangeState(State.LOADLEVEL, 2f);
                }
                break;
            case State.LOADLEVEL:
                StopTimer();
                BeginTimer();
                updateTextLevel();
                if (currentUser.Level > levels.Length)
                {
                    ChangeState(State.GAMEOVER);
                }
                else
                {
                    currentLevel = Instantiate(levels[currentUser.Level - 1]);
                    ChangeState(State.PLAY);
                }
                break;
            case State.GAMEOVER:
                StopTimer();
                formattedTime = TimeSpan.FromSeconds(totalTime).ToString("mm':'ss'.'ff");
                currentUser.Time = formattedTime;
                if (currentLevel.name != "Tutorial" && !isReplay)
                {
                    SaveManager.SaveUser(currentUser, replay);
                }
                textTotalTime.text = "Total time: " + formattedTime;
                textHighestLevel.text = "Level: " + currentUser.Level;
                textTotalScore.text = "Score: " + currentUser.Score;
                panelGameOver.SetActive(true);
                textAllLevelsCompleted.enabled = false;
                textGameOverTitle.enabled = true;
                ChangeState(State.MAIN_MENU, 3f);
                break;
            case State.GAME_COMPLETED:
                SoundManager.PlaySound("Game Completed");
                StopTimer();
                formattedTime = TimeSpan.FromSeconds(totalTime).ToString("mm':'ss'.'ff");
                currentUser.Time = formattedTime;
                if (currentLevel.name != "Tutorial" && !isReplay)
                {
                    SaveManager.SaveUser(currentUser, replay);
                }
                textTotalTime.text = "Total time: " + formattedTime;
                textHighestLevel.text = "Level: " + currentUser.Level;
                textTotalScore.text = "Score: " + currentUser.Score;
                panelGameOver.SetActive(true);
                textGameOverTitle.enabled = false;
                textAllLevelsCompleted.enabled = true;
                ChangeState(State.MAIN_MENU, 3f);
                if (currentUser.Lives == 2)
                {
                    AchievementManager.Instance.NotifyAchievementComplete(9);
                }
                else
                {
                    AchievementManager.Instance.NotifyAchievementComplete(8);
                }
                break;
            case State.INIT_TUTORIAL:
                Invoke("InitTutorialDelay", 1f);
                break;
            case State.TUTORIAL:
                Notify("PanelMovePlatform");
                break;
            case State.INIT_REPLAY:
                panelPlay.SetActive(true);
                currentUser = new User("Replay", 2, 0, 1);
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
                ProfileDropdownManager.Instance.FillDropdownWithUsers();
                break;
            case State.GAME_COMPLETED:
                panelPlay.SetActive(false);
                panelGameOver.SetActive(false);
                Destroy(platform);
                Destroy(currentLevel);
                ProfileDropdownManager.Instance.FillDropdownWithUsers();
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
        isSwitchingState = true;
        yield return new WaitForSeconds(delay);
        EndState();
        state = newState;
        BeginState(newState);
        isSwitchingState = false;
    }

    private void ProfileMenuDelay()
    {
        panelProfileMenu.SetActive(true);
        textUsernameError.enabled = false;
        inputFieldNewProfile.text = "Enter name...";
        PlayProfileSelectMenuAnimation("ProfileSelect_Start");
    }

    private void MainMenuDelay()
    {
        Cursor.visible = true;
        panelMenu.SetActive(true);
        PlayMainMenuAnimation("Menu_Start");
    }

    private void InitDelay()
    {
        Cursor.visible = false;
        panelPlay.SetActive(true);
        currentUser = new User(inputFieldNewProfile.text, 2, 0, 1);
        inputFieldNewProfile.text = " ";
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
        currentUser = new User("Tutorial", 999, 0, 1);
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
        textLevel.text = "Level: " + currentUser.Level;
    }

    public void StartGameClicked()
    {
        ChangeState(State.INIT);
    }

    public void TutorialClicked()
    {
        SoundManager.PlaySound("Button Clicked");
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
        Cursor.visible = true;
        SoundManager.PlaySound("Button Clicked");
        ball.BroadcastMessage("PauseBall");
        StopTimer();
    }

    public void UnpauseGameAndLaunchBallAfterOneSecond()
    {
        Cursor.visible = false;
        ball.BroadcastMessage("LaunchBallAfterOneSecond");
        StartTimer();
    }

    public void ProfileSelectClicked()
    {
        SoundManager.PlaySound("Button Clicked");
        ChangeState(State.PROFILE_MENU);
    }

    public void BackToMainMenuClicked()
    {
        SoundManager.PlaySound("Button Clicked");
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
        SoundManager.PlaySound("Button Clicked");
        if (SaveManager.GetUsers().Count > 0)
        {
            replay = SaveManager.LoadReplay(ProfileDropdownManager.Instance.GetCurrentUser().Name);
            isReplay = true;
            ChangeState(State.INIT_REPLAY);
        }
    }

    public void InputOnValueChanged(string name)
    {
        if (inputFieldNewProfile.text.Equals(""))
        {
            textUsernameError.text = "Please input a name in the field";
            textUsernameError.enabled = true;
            buttonStartGame.interactable = false;
        }
        else
        {
            //If the username is taken show an error and dont allow the game to be started
            if (SaveManager.DoesUserExist(inputFieldNewProfile.text))
            {
                textUsernameError.text = "Username is taken";
                textUsernameError.enabled = true;
                buttonStartGame.interactable = false;
            }
            else
            {
                textUsernameError.enabled = false;
                buttonStartGame.interactable = true;
            }
        }
    }

    public void ExitApplication()
    {
        SoundManager.PlaySound("Button Clicked");
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
            if (!panelPause.activeSelf)
            {
                isPaused = true;
                PauseGame();
                panelPause.SetActive(true);
            }
        }
    }

    public void GoToMainMenuAndCloseGame()
    {
        SoundManager.PlaySound("Button Clicked");
        ChangeState(State.MAIN_MENU);
        isPaused = false;
        StopTimer();
        panelPause.SetActive(false);
        panelPlay.SetActive(false);
        Invoke(nameof(DestroyLevelObjects), 0.05f);
    }

    private void DestroyLevelObjects()
    {
        Destroy(platform);
        Destroy(currentLevel);
        Destroy(ball);
    }

    public void ResumeGame()
    {
        Cursor.visible = false;
        SoundManager.PlaySound("Button Clicked");
        StartTimer();
        isPaused = false;
        panelPause.SetActive(false);
        ball.BroadcastMessage("UnpauseBall");
    }

    public bool GetIsPaused()
    {
        return isPaused;
    }

    public void AchievementsCanvasChangeVisibility()
    {
        SoundManager.PlaySound("Button Clicked");
        if (SaveManager.GetUsers().Count > 0)
        {
            if (canvasUserAchievements.GetComponent<CanvasGroup>().alpha == 0)
            {
                canvasUserAchievements.GetComponent<CanvasGroup>().alpha = 1;
            }
            else
            {
                canvasUserAchievements.GetComponent<CanvasGroup>().alpha = 0;
            }
            //canvasUserAchievements.SetActive(!canvasUserAchievements.activeSelf);
            panelUserStats.SetActive(!panelUserStats.activeSelf);
        }
    }

    public void SetAchievementsAndReplayButtonsActive()
    {
        buttonReplay.GetComponent<CanvasGroup>().alpha = 1;
        buttonAchievements.GetComponent<CanvasGroup>().alpha = 1;
    }
}
