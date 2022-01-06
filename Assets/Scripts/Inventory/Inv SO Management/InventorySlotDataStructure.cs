using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class IntPair
{
    [Tooltip("Flipped :("), Min(1)]
    public int x;
    [Tooltip("Flipped :("), Min(1)]
    public int y;

    public IntPair(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}

[System.Serializable]
public class FloatPair
{
    public float x;
    public float y;

    public FloatPair(float x, float y)
    {
        this.x = x;
        this.y = y;
    }
}