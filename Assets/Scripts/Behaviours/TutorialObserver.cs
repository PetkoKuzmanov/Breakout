using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        Instance = this;

        buttonContinue = GameObject.Find("Canvas/CanvasTutorial/ButtonContinue");
        tutorialPanels.Add(GameObject.Find("Canvas/CanvasTutorial/PanelMovePlatform"));
        tutorialPanels.Add(GameObject.Find("Canvas/CanvasTutorial/PanelBall"));
        tutorialPanels.Add(GameObject.Find("Canvas/CanvasTutorial/PanelBrick0"));
        tutorialPanels.Add(GameObject.Find("Canvas/CanvasTutorial/PanelBrick1"));
        tutorialPanels.Add(GameObject.Find("Canvas/CanvasTutorial/PanelBrick2"));
        tutorialPanels.Add(GameObject.Find("Canvas/CanvasTutorial/PanelBallDeath"));

    }

    public void Update()
    {

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


    public void ContinueClicked()
    {
        GameManager.Instance.UnpauseGame();
        buttonContinue.gameObject.SetActive(false);
        if (tutorialPanels[0].activeSelf)
        {
            CallShowTutorialPanelWithDelay(tutorialPanels[1], 1);
            tutorialPanels[0].SetActive(false);
        }
        else if (tutorialPanels[2].activeSelf)
        {
            CallShowTutorialPanelWithDelay(tutorialPanels[3]);
            tutorialPanels[2].SetActive(false);
        }
        else if (tutorialPanels[3].activeSelf)
        {
            CallShowTutorialPanelWithDelay(tutorialPanels[4]);
            tutorialPanels[3].SetActive(false);
        }
        else
        {
            foreach (GameObject panel in tutorialPanels)
            {
                panel.SetActive(false);
            }
        }
    }
}
