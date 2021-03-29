using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopulateGrid : MonoBehaviour
{
	public GameObject prefab; // This is our prefab object that will be exposed in the inspector

	//public int numberToCreate; // number of objects to create. Exposed in inspector

	public List<Achievement> listOfAchievements = new List<Achievement>();

	private Sprite sprite;

	void Start()
	{
		Populate();
	}

	void Update()
	{

	}

	void Populate()
	{
		GameObject newObj; // Create GameObject instance

		//for (int i = 0; i < numberToCreate; i++)
		//{
		//	// Create new instances of our prefab until we've created as many as we specified
		//	newObj = (GameObject)Instantiate(prefab, transform);

		//	// Randomize the color of our image
		//	//newObj.GetComponent<Image>().color = Random.ColorHSV();
		//	//newObj.GetComponent<RectTransform>().GetChild(0).GetComponent<Image>().color = Random.ColorHSV();
		//}

		foreach(Achievement achievement in listOfAchievements)
        {
			newObj = (GameObject)Instantiate(prefab, transform);

			sprite = Sprite.Create(achievement.texture, new Rect(0.0f, 0.0f, achievement.texture.width, achievement.texture.height), new Vector2(0.5f, 0.5f), 100.0f);

			newObj.GetComponent<RectTransform>().GetChild(0).GetComponent<Image>().sprite = sprite;
			newObj.GetComponent<RectTransform>().GetChild(1).GetComponent<TextMeshProUGUI>().text = achievement.text;
		}

	}
}
