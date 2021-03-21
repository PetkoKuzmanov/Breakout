using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;


public static class SaveManager
{
    private static ArrayList users;

    public static void SaveUser(User user)
    {
        BinaryFormatter formatter = new BinaryFormatter();

        string fileName = user.Name + ".dat";
        string path = Path.Combine(Application.persistentDataPath, fileName);
        FileStream stream = new FileStream(path, FileMode.Create);

        formatter.Serialize(stream, user);
        stream.Close();
    }

    public static void LoadUser(string path)
    {
        if (File.Exists(path))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            User user = formatter.Deserialize(stream) as User;
            stream.Close();

            users.Add(user);
        }
        else
        {
            Debug.LogError("Save file not found in " + path);
        }
    }

    public static void LoadUsers()
    {
        string path = Application.persistentDataPath;
        string[] files = Directory.GetFiles(path);

        for (int i = 0; i < files.Length; i++)
        {
            LoadUser(files[i]);
        }
    }
}
