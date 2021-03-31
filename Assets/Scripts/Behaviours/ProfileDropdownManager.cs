using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileDropdownManager : MonoBehaviour
{
    public static ProfileDropdownManager Instance { get; private set; }

    private TMP_Dropdown dropdown;
    private ArrayList userList;

    public GameObject panelUserStats;
    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textTime;
    public TextMeshProUGUI textLives;
    public TextMeshProUGUI textLevel;

    public GameObject achievementsScrollView;

    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        dropdown = GetComponent<TMP_Dropdown>();

        FillDropdownWithUsers();

        //Call the function initially

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }


    private void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        //Show the info for the selected user
        if (userList.Count > 0)
        {
            GameManager.Instance.SetAchievementsAndReplayButtonsActive();
            int index = dropdown.value;
            User currentUser = userList[index] as User;

            textScore.text = currentUser.Score.ToString();
            textTime.text = currentUser.Time.ToString();
            textLives.text = currentUser.Lives.ToString();
            textLevel.text = currentUser.Level.ToString();

            SetUpAchievementsWindow(currentUser);
            panelUserStats.SetActive(true);
        }
    }

    public User GetCurrentUser()
    {
        int index = dropdown.value;
        User currentUser = userList[index] as User;

        return currentUser;
    }

    public void FillDropdownWithUsers()
    {
        dropdown.options.Clear();
        //for (int i = 0; i < dropdown.options.Count; i++)
        //{
        //    dropdown.options.RemoveAt(i);
        //}
        userList = SaveManager.LoadUsers();

        foreach (User user in userList)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = user.Name });
        }

        DropdownItemSelected(dropdown);
    }

    private void SetUpAchievementsWindow(User currentUser)
    {
        for (int i = 0; i < User.NumberOfAchievements; i++)
        {
            //If the achievement is unlocked set its alpha to 1 otherwise to 0.5
            if (currentUser.Achievements[i])
            {
                achievementsScrollView.transform.GetChild(i).gameObject.GetComponent<CanvasGroup>().alpha = 1f;
            }
            else
            {
                achievementsScrollView.transform.GetChild(i).gameObject.GetComponent<CanvasGroup>().alpha = 0.5f;
            }
        }
    }
}
