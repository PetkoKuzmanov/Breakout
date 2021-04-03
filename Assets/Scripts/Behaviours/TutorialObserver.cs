using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    public void OnNotify(string notificationName);
}
public class TutorialObserver : MonoBehaviour, IObserver
{
    public static TutorialObserver Instance { get; private set; }

    private GameObject buttonContinue;

    private List<GameObject> tutorialPanels = new List<GameObject>();

    public void Start()
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

        buttonContinue = GameObject.Find("Canvas/PanelTutorial/ButtonContinue");
        tutorialPanels.Add(GameObject.Find("Canvas/PanelTutorial/PanelMovePlatform"));
        tutorialPanels.Add(GameObject.Find("Canvas/PanelTutorial/PanelBall"));
        tutorialPanels.Add(GameObject.Find("Canvas/PanelTutorial/PanelBrick0"));
        tutorialPanels.Add(GameObject.Find("Canvas/PanelTutorial/PanelBrick1"));
        tutorialPanels.Add(GameObject.Find("Canvas/PanelTutorial/PanelBrick2"));
        tutorialPanels.Add(GameObject.Find("Canvas/PanelTutorial/PanelBallDeath"));
    }

    public void OnNotify(string notificationName)
    {
        switch (notificationName)
        {
            case "PanelMovePlatform":
                CallShowTutorialPanelWithDelay(tutorialPanels[0], 1f);
                break;
            case "PanelBall":
                CallShowTutorialPanelWithDelay(tutorialPanels[1], 1f);
                break;
            case "PanelBrick0":
                CallShowTutorialPanelWithDelay(tutorialPanels[2]);
                break;
            case "PanelBrick1":
                CallShowTutorialPanelWithDelay(tutorialPanels[3]);
                break;
            case "PanelBrick2":
                CallShowTutorialPanelWithDelay(tutorialPanels[4]);
                break;
            case "PanelBallDeath":
                CallShowTutorialPanelWithDelay(tutorialPanels[5]);
                break;

        }
    }

    private void CallShowTutorialPanelWithDelay(GameObject panel, float delay = 0)
    {
        StartCoroutine(ShowTutorialPanelWithDelay(panel, delay));
    }

    IEnumerator ShowTutorialPanelWithDelay(GameObject panel, float delay)
    {
        yield return new WaitForSeconds(delay);
        panel.SetActive(true);
        buttonContinue.gameObject.SetActive(true);
        GameManager.Instance.PauseGame();
    }

    private void ContinueBrickTutorial(GameObject panel)
    {
        buttonContinue.gameObject.SetActive(true);
        panel.SetActive(true);
    }

    public void ContinueClicked()
    {
        SoundManager.PlaySound("Button Clicked");
        buttonContinue.gameObject.SetActive(false);

        if (tutorialPanels[0].activeSelf)
        {
            CallShowTutorialPanelWithDelay(tutorialPanels[1], 1);
            tutorialPanels[0].SetActive(false);
        }
        else if (tutorialPanels[2].activeSelf)
        {
            ContinueBrickTutorial(tutorialPanels[3]);
            tutorialPanels[2].SetActive(false);
        }
        else if (tutorialPanels[3].activeSelf)
        {
            ContinueBrickTutorial(tutorialPanels[4]);
            tutorialPanels[3].SetActive(false);
        }
        else if (tutorialPanels[4].activeSelf)
        {
            GameManager.Instance.ResumeGame();
            tutorialPanels[4].SetActive(false);
        }
        else
        {
            GameManager.Instance.UnpauseGameAndLaunchBallAfterOneSecond();
            foreach (GameObject panel in tutorialPanels)
            {
                panel.SetActive(false);
            }
        }
    }
}
