using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public static class SaveManager
{
    private static ArrayList users = new ArrayList();

    public static void SaveUser(User user, List<float> replay)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string fileName = user.Name + ".dat";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, user);
        stream.Close();

        SaveReplay(replay, user.Name);
    }

    public static void LoadUser(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);
            stream.Position = 0;

            User user = formatter.Deserialize(stream) as User;
            stream.Close();
            users.Add(user);
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
        }
    }

    public static ArrayList LoadUsers()
    {
        string path = Application.persistentDataPath;
        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            LoadUser(files[i]);
        }

        return users;
    }

    public static ArrayList GetUsers()
    {
        return users;
    }

    public static void SaveReplay(List<float> replay, string name)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string fileName = name + "_replay.dat";

        string folderPath = Path.Combine(Application.persistentDataPath, "replays");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string filePath = Path.Combine(folderPath, fileName);
        FileStream stream = new FileStream(filePath, FileMode.Create);

        formatter.Serialize(stream, replay);
        stream.Close();
    }

    public static List<float> LoadReplay(string username)
    {
        string folderPath = Path.Combine(Application.persistentDataPath, "replays");
        string filePath = Path.Combine(folderPath, username + "_replay.dat");

        if (File.Exists(filePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(filePath, FileMode.Open);
            stream.Position = 0;

            List<float> replay = formatter.Deserialize(stream) as List<float>;
            stream.Close();
            return replay;
        }
        else
        {
            Debug.LogError("Replay file not found in " + filePath);
            return null;
        }
    }

    public static bool DoesUserExist(string username)
    {
        foreach (User user in users)
        {
            if (user.Name == username)
            {
                return true;
            }
        }

        return false;
    }
}
