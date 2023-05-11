using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

using UnityEngine.Tilemaps;
public class Ghost : MonoBehaviour
{
    public Tile tile;

    public Board board;

    public Piece trackingPiece;
    public Tilemap tilemap { get; private set; }
    public Vector3Int[] cells { get; private set; }
    public Vector3Int position { get; private set; }
    // Start is called before the first frame update
    private void Awake()
    {
        tilemap = GetComponentInChildren<Tilemap>();
        cells = new Vector3Int[4];
    }

    private void LateUpdate() //make sure ghost updates after main pieace
    {
        Clear();
        Copy();
        Drop();
        Set();
    }

    private void Clear()
    {
        for (int i=0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, null);
        }
    }

    private void Copy()
    {
        for (int i=0; i < cells.Length; i++)
        {
            cells[i] = trackingPiece.Cells[i];
        }
    }

    private void Drop()
    {
        Vector3Int pos = trackingPiece.Position;
        int currentRow = pos.y;
        int bottom = -board.boardSize.y / 2 - 1;
        
        board.Clear(trackingPiece); //need to clear tracking piece sine it's occupyin the space
        for (int row = currentRow; row >= bottom; row--)
        {
            pos.y = row;
            if (board.IsValidPosition(trackingPiece, pos))
            {
                position = pos;
            }
            else
            {
                break;
            }
        }
        board.Set(trackingPiece);
    }

    private void Set()
    {
        for (int i=0; i < cells.Length; i++)
        {
            Vector3Int tilePosition = cells[i] + position;
            tilemap.SetTile(tilePosition, tile);
        }
    }
}

