using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopulateUserAchievements : MonoBehaviour
{
    public GameObject prefab; // This is our prefab object that will be exposed in the inspector

    //public int numberToCreate; // number of objects to create. Exposed in inspector

    private List<Achievement> listOfAchievements = new List<Achievement>();

    private Sprite sprite;

    private void Awake()
    {
        listOfAchievements = AchievementManager.Instance.achievements;
    }

    void Start()
    {
        Populate();
    }

    void Update()
    {

    }

    public void Populate()
    {
        GameObject newObj; // Create GameObject instance

        foreach (Achievement achievement in listOfAchievements)
        {
            newObj = (GameObject)Instantiate(prefab, transform);

            sprite = Sprite.Create(achievement.texture, new Rect(0.0f, 0.0f, achievement.texture.width, achievement.texture.height), new Vector2(0.5f, 0.5f), 100.0f);

            newObj.GetComponent<RectTransform>().GetChild(0).GetComponent<Image>().sprite = sprite;
            newObj.GetComponent<RectTransform>().GetChild(1).GetComponent<TextMeshProUGUI>().text = achievement.text;
        }

    }
}
