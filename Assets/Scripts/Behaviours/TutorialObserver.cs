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
    private GameObject panelMovePlatform;

    public void Start()
    {
        Instance = this;
        buttonContinue = GameObject.Find("Canvas/CanvasTutorial/ButtonContinue");
        panelMovePlatform = GameObject.Find("Canvas/CanvasTutorial/PanelMovePlatform");
    }

    public void Update()
    {

    }
    public void OnNotify(string notificationName)
    {
        switch (notificationName)
        {
            case "PanelMovePlatform":
                //panelMovePlatform.SetActive(true);
                CallShowTutorialPanelWithDelay(panelMovePlatform, 1f);
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
        //tutorialPanels[index].SetActive(true);
        buttonContinue.gameObject.SetActive(true);
        GameManager.Instance.PauseGame();
    }
}
