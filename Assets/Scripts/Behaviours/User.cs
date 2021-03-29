using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class User
{
    private static int NUMBER_OF_ACHIEVEMENTS = 12;

    public string name;
    public int score;
    public int level;
    public int lives;
    public string time;

    //public List<bool> achievementsUnlocked = new List<bool>();
    
    public bool[] achievementsUnlocked = new bool[NUMBER_OF_ACHIEVEMENTS];

    public User(string name, int lives, int score, int level)
    {
        this.name = name;
        this.lives = lives;
        this.score = score;
        this.level = level;
    }

    public User(string name, int lives, int score, int level, string time)
    {
        this.name = name;
        this.lives = lives;
        this.score = score;
        this.level = level;
        this.time = time;
    }

    public int Score
    {
        get { return score; }
        set { score = value; }
    }

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public int Lives
    {
        get { return lives; }
        set { lives = value; }
    }

    public string Name
    {
        get { return name; }
    }

    public string Time
    {
        get { return time; }
        set { time = value; }
    }

    public bool[] AchievementsUnlocked
    {
        get { return achievementsUnlocked; }
        set { achievementsUnlocked = value; }
    }

    public static int NumberOfAchievements
    {
        get { return NUMBER_OF_ACHIEVEMENTS; }
    }
}
