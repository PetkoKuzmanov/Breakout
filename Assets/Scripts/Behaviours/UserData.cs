using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserData
{
    private string name;
    private int score;
    private int level;
    private int lives;
    private string[] levelTimes;

    public UserData(User user)
    {
        name = user.Name;
        score = user.Score;
        level = user.Level;
        lives = user.Lives;
        levelTimes = user.LevelTimes;
    }
}
