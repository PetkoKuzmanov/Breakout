using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ReplayPosition
{
    private float[] position = new float[2];

    public ReplayPosition(Vector2 vector)
    {
        position[0] = vector.x;
        position[1] = vector.y;
    }

    public float GetX()
    {
        return position[0];
    }

    public float GetY()
    {
        return position[1];
    }
}
