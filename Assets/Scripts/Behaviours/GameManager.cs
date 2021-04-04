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
        if (!Instance)
        {
            Instance = this;
        }
        observers.Add(TutorialObserver.Instance);
        panelMenuAnimator = panelMenu.GetComponent<Animator>();
        panelProfileSelectAnimator = panelProfileMenu.GetComponent<Animator>();
        ChangeState(State.MAIN_MENU);
    }

    private void Update()
    {
        switch (state)
        {
            case State.PLAY:
            case State.TUTORIAL:
                PauseGameIfEscapePressed();
                break;
        }
    }

    void FixedUpdate()
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
                FixedUpdatePlay();
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
                FixedUpdateTutorial();
                break;
        }
    }

    void BeginState(State newState)
    {
        switch (newState)
        {
            case State.MAIN_MENU:
                Invoke(nameof(MainMenuBegin), 1f);
                break;
            case State.PROFILE_MENU:
                Invoke(nameof(ProfileMenuBegin), 1f);
                break;
            case State.INIT:
                Invoke(nameof(InitBegin), 1f);
                break;
            case State.PLAY:
                break;
            case State.LEVELCOMPLETED:
                BeginStateLevelCompleted();
                break;
            case State.LOADLEVEL:
                BeginStateLoadLevel();
                break;
            case State.GAMEOVER:
                BeginStateGameover();
                break;
            case State.GAME_COMPLETED:
                BeginStateGameCompleted();
                break;
            case State.INIT_TUTORIAL:
                Invoke(nameof(InitTutorialBegin), 1f);
                break;
            case State.TUTORIAL:
                Notify("PanelMovePlatform");
                break;
            case State.INIT_REPLAY:
                Invoke(nameof(InitReplay), 1f);
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
                EndStateEndOfGameSetup();
                break;
            case State.GAME_COMPLETED:
                EndStateEndOfGameSetup();
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

    private IEnumerator ChangeDelay(State newState, float delay)
    {
        isSwitchingState = true;
        yield return new WaitForSeconds(delay);
        EndState();
        state = newState;
        BeginState(newState);
        isSwitchingState = false;
    }

    private void ProfileMenuBegin()
    {
        panelProfileMenu.SetActive(true);
        textUsernameError.enabled = false;
        inputFieldNewProfile.text = "Enter name...";
        PlayProfileSelectMenuAnimation("ProfileSelect_Start");
    }

    private void MainMenuBegin()
    {
        Cursor.visible = true;
        panelMenu.SetActive(true);
        PlayMainMenuAnimation("Menu_Start");
    }

    private void InitBegin()
    {
        Cursor.visible = false;
        panelPlay.SetActive(true);
        currentUser = new User(inputFieldNewProfile.text, 2, 0, 1);
        inputFieldNewProfile.text = " ";
        UpdateAllStats();
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
        platform = Instantiate(platformPrefab);
        ChangeState(State.LOADLEVEL);
    }

    private void InitTutorialBegin()
    {
        panelPlay.SetActive(true);
        currentUser = new User("Tutorial", 999, 0, 1);
        UpdateAllStats();
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

    private void InitReplay()
    {
        panelPlay.SetActive(true);
        currentUser = new User("Replay", 2, 0, 1);
        UpdateAllStats();
        if (currentLevel != null)
        {
            Destroy(currentLevel);
        }
        platform = Instantiate(platformPrefab);
        ChangeState(State.LOADLEVEL);
    }

    private void FixedUpdatePlay()
    {
        if (isReplay)
        {
            //Replay the game
            PlayerMove.Instance.SetVelocityFromReplay(replay[replayCount]);
            replayCount++;
        }
        else
        {
            if (!isPaused)
            {
                //Write to the replay list
                replay.Add(PlayerMove.Instance.GetDirection());
            }
        }

        //Check if the ball has fallen out of the map
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

        //If the current level is null then it is completed
        if (currentLevel != null && currentLevel.transform.childCount == 0 && !isSwitchingState)
        {
            ChangeState(State.LEVELCOMPLETED);
        }
    }

    private void FixedUpdateTutorial()
    {
        //Check if the ball has fallen out of the map
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

        //If the current level is null then end the tutorial
        if (currentLevel != null && currentLevel.transform.childCount == 0 && !isSwitchingState)
        {
            ChangeState(State.GAMEOVER);
        }
        PauseGameIfEscapePressed();
    }

    private void BeginStateLevelCompleted()
    {
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
    }

    private void BeginStateLoadLevel()
    {
        StopTimer();
        BeginTimer();
        UpdateTextLevel();
        if (currentUser.Level > levels.Length)
        {
            ChangeState(State.GAMEOVER);
        }
        else
        {
            currentLevel = Instantiate(levels[currentUser.Level - 1]);
            ChangeState(State.PLAY);
        }
    }

    private void BeginStateGameover()
    {
        BeginStateEndOfGameSetup();
        textAllLevelsCompleted.enabled = false;
        textGameOverTitle.enabled = true;
        ChangeState(State.MAIN_MENU, 3f);
    }

    private void BeginStateGameCompleted()
    {
        BeginStateEndOfGameSetup();
        SoundManager.PlaySound("Game Completed");
        textGameOverTitle.enabled = false;
        textAllLevelsCompleted.enabled = true;
        if (currentUser.Lives == 2)
        {
            AchievementManager.Instance.NotifyAchievementComplete(9);
        }
        else
        {
            AchievementManager.Instance.NotifyAchievementComplete(8);
        }
        ChangeState(State.MAIN_MENU, 3f);
    }

    private void BeginStateEndOfGameSetup()
    {
        //Stop the timer and save it
        StopTimer();
        formattedTime = TimeSpan.FromSeconds(totalTime).ToString("mm':'ss'.'ff");
        currentUser.Time = formattedTime;

        //Save the current user
        if (currentUser.Name != "Tutorial" && !isReplay)
        {
            SaveManager.SaveUser(currentUser, replay);
        }

        //Stop the replay saving
        isReplay = false;
        replayCount = 0;
        replay.Clear();

        //Setup the panel
        textTotalTime.text = "Total time: " + formattedTime;
        textHighestLevel.text = "Level: " + currentUser.Level;
        textTotalScore.text = "Score: " + currentUser.Score;
        panelGameOver.SetActive(true);
    }

    private void EndStateEndOfGameSetup()
    {
        panelPlay.SetActive(false);
        panelGameOver.SetActive(false);
        Destroy(platform);
        Destroy(currentLevel);
        ProfileDropdownManager.Instance.FillDropdownWithUsers();
    }

    public User GetCurrentUser()
    {
        return currentUser;
    }

    public void UpdateTextScore()
    {
        textScore.text = "Score: " + currentUser.Score;
    }

    public void UpdateTextLives()
    {
        textLives.text = "Lives: " + currentUser.Lives;
    }

    public void UpdateTextLevel()
    {
        textLevel.text = "Level: " + currentUser.Level;
    }

    public void UpdateAllStats()
    {
        UpdateTextScore();
        UpdateTextLives();
        UpdateTextLevel();
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
