
[System.Serializable]
public class User
{
    private static int numberOfAchievements = 12;

    private string name;
    private int score;
    private int level;
    private int lives;
    private string time;

    private int platformHitCounter;
    private int bricksDestroyed;
    private int redBricksDestroyed;
    private int yellowBricksDestroyed;
    private int blueBricksDestroyed;

    public bool[] achievements = new bool[numberOfAchievements];

    public User(string name, int lives, int score, int level)
    {
        this.name = name;
        this.lives = lives;
        this.score = score;
        this.level = level;
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

    public bool[] Achievements
    {
        get { return achievements; }
        set { achievements = value; }
    }

    public static int NumberOfAchievements
    {
        get { return numberOfAchievements; }
    }

    public void UnlockAchievement(int ID)
    {
        achievements[ID] = true;
    }

    public int GetBricksDestroyed()
    {
        return bricksDestroyed;
    }

    public void IncrementBricksDestroyed()
    {
        bricksDestroyed++;
    }

    public int GetPlatformHitCounter()
    {
        return platformHitCounter;
    }

    public void IncrementPlatformHitCounter()
    {
        platformHitCounter++;
    }

    public int GetRedBricksDestroyed()
    {
        return redBricksDestroyed;
    }

    public void IncrementRedBricksDestroyed()
    {
        redBricksDestroyed++;
    }

    public int GetYellowBricksDestroyed()
    {
        return yellowBricksDestroyed;
    }

    public void IncrementYellowBricksDestroyed()
    {
        yellowBricksDestroyed++;
    }

    public int GetBlueBricksDestroyed()
    {
        return blueBricksDestroyed;
    }

    public void IncrementBlueBricksDestroyed()
    {
        blueBricksDestroyed++;
    }
}
