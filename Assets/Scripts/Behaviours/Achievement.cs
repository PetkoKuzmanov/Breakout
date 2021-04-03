using UnityEngine;

[CreateAssetMenu(fileName = "Achievement", menuName = "ScriptableObjects/Achievement", order = 1)]
public class Achievement : ScriptableObject
{
    public int ID;
    public Texture2D texture;
    public string text;
}
