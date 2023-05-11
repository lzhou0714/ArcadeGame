using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z
}
[System.Serializable]
public struct TData
{
    public Tetromino tetromino;
    public Tile tile;

    //holds tetromino structure info
    public Vector2Int[] cells { get; private set; }
    public Vector2Int[,] Wallkicks { get; private set; }
    public int Effect { get; private set; }

    public void Initialize()
    {
        //assign shape based on our tetromino data
        cells = Data.Cells[tetromino];
        Wallkicks = Data.WallKicks[tetromino];
        Effect = Data.Effects[tetromino];
    }
}
