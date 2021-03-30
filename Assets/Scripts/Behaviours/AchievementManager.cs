using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    private Queue<Achievement> achievementQueue = new Queue<Achievement>();

    public List<Achievement> achievements = new List<Achievement>();

    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        StartCoroutine(AchievementQueueCheck());
    }

    public void NotifyAchievementComplete(int ID)
    {
        Debug.Log(ID);
        //find the achievement and enqueue it
        //achievementQueue.Enqueue()
    }

    private void UnlockAchievement(Achievement achievement)
    {
        //Instantiate the achievement on the screen and set the achievement true for the user
    }

    private IEnumerator AchievementQueueCheck()
    {
        for (; ; )
        {
            if (achievementQueue.Count > 0) UnlockAchievement(achievementQueue.Dequeue());
            yield return new WaitForSeconds(2f);
        }
    }
}
