using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[System.Serializable]
public class User
{
    public string name;
    public int score;
    public int level;
    public int lives;
    public string time;

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
}
