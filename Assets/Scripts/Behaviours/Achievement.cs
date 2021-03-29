using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Achievement", menuName = "ScriptableObjects/Achievement", order = 1)]
public class Achievement : ScriptableObject
{
    // Start is called before the first frame update

    public Texture2D texture;
    public string text;
}
