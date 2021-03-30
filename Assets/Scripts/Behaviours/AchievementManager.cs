using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance { get; private set; }

    private Queue<Achievement> achievementQueue = new Queue<Achievement>();

    public List<Achievement> achievementList = new List<Achievement>();

    public GameObject prefab;
    private Sprite sprite;
    private GameObject achievementObject;

    private Vector3 position = new Vector3(0, -200, 0);

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        StartCoroutine(AchievementQueueCheck());
    }

    public void NotifyAchievementComplete(int ID)
    {
        achievementQueue.Enqueue(achievementList[ID]);
    }

    private void UnlockAchievement(Achievement achievement)
    {
        //Instantiate the achievement on the screen and set the achievement true for the user
        GameManager.Instance.getCurrentUser().UnlockAchievement(achievement.ID);

        achievementObject = (GameObject)Instantiate(prefab, transform);

        sprite = Sprite.Create(achievement.texture, new Rect(0.0f, 0.0f, achievement.texture.width, achievement.texture.height), new Vector2(0.5f, 0.5f), 100.0f);

        achievementObject.GetComponent<RectTransform>().GetChild(0).GetComponent<Image>().sprite = sprite;
        achievementObject.GetComponent<RectTransform>().GetChild(1).GetComponent<TextMeshProUGUI>().text = achievement.text;

        Invoke(nameof(DestroyAchievement), 2f);
    }

    private void DestroyAchievement()
    {
        Destroy(achievementObject);
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
