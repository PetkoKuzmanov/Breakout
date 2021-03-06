using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class User
{

    private string name;
    private int[] levels;
    private string[] times;
    private int[] scores;
    private int totalScore;


    public User(string name, int lives)
    {
        this.name = name;
        this.lives = lives;
    }


    private int score;
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
        }
    }

    private int level;
    public int Level
    {
        get { return level; }
        set
        {
            level = value;
        }
    }

    private int lives;
    public int Lives
    {
        get { return lives; }
        set
        {
            lives = value;
        }
    }

}
