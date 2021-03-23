using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ProfileDropdownManager : MonoBehaviour
{
    private TMP_Dropdown dropdown;
    private ArrayList userList;

    public TextMeshProUGUI textScore;
    public TextMeshProUGUI textTime;
    public TextMeshProUGUI textLives;
    public TextMeshProUGUI textLevel;

    // Start is called before the first frame update
    void Start()
    {
        dropdown = GetComponent<TMP_Dropdown>();
        dropdown.options.Clear();

        userList = SaveManager.LoadUsers();

        foreach(User user in userList)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData() { text = user.Name });
        }

        //Call it initially
        DropdownItemSelected(dropdown);

        dropdown.onValueChanged.AddListener(delegate { DropdownItemSelected(dropdown); });
    }


    private void DropdownItemSelected(TMP_Dropdown dropdown)
    {
        //Show the info for the player
        int index = dropdown.value;
        Debug.Log(index);

        User currentUser = userList[index] as User;

        textScore.text = currentUser.Score.ToString();
        textTime.text = currentUser.Time.ToString();
        textLives.text = currentUser.Lives.ToString();
        textLevel.text = currentUser.Level.ToString();
    }
}
