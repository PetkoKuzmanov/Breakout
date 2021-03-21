using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class UserData
{
    public string name;
    public int score;
    public int level;
    public int lives;
    public string[] levelTimes;

    public UserData(User user)
    {
        name = user.Name;
        score = user.Score;
        level = user.Level;
        lives = user.Lives;
        levelTimes = user.LevelTimes;
    }
}
