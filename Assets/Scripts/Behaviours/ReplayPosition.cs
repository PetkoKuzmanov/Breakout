using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReplayPosition
{
    public float[] position = new float[2];

    public ReplayPosition(Vector2 vector)
    {
        position[0] = vector.x;
        position[1] = vector.y;
    }
}
