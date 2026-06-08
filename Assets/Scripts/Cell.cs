// from https://github.com/zigurous/unity-minesweeper-tutorial

using UnityEngine;

public class Cell
{
    public enum Type
    {
        Empty,
        Antlion,
        Number,
    }

    public Vector3Int position;
    public Type type;
    public int number;
    public bool revealed;
    public bool exploded;
}