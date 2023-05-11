using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;


public class Piece : MonoBehaviour
{
    public int playerIndex;

    public bool isSpecial = false;
    public Board Board { get; private set; }

    public TData data  { get; private set; }
    
    //array containing locations of the tiles that make up the piece
    public Vector3Int[] Cells  { get; private set; }
    public Vector3Int Position { get; private set; } //tilemap uses vector3int?
    private Vector2 moveDir;
    public int rotationInd { get; private set; }
    public float stepDelay = 1f;
    public float lockDelay = 0.4f;
    
    private float stepTime;
    private float lockTime;
    public void Initialize(Board board, Vector3Int position, TData tdata)
    {
        //where to spawn piece
        // what tetromino data to use
        
        Board = board;
        Position = position;
        data = tdata;
        rotationInd = 0;
        stepTime = Time.time + this.stepDelay;
        lockTime = 0f;
        if (Random.Range(0f, 1f) < Board.pSpecial)
        {
            isSpecial = true;
        }
        else
        {
            isSpecial = false;
            isSpecial = false;
        }

        if (Cells == null)
        {
            Cells = new Vector3Int[data.cells.Length];
        }

        for (int i = 0; i < data.cells.Length; i++)
        {
            Cells[i] = (Vector3Int) data.cells[i];
        }
    }

    public void Update()
    {
        Board.Clear(this);
        lockTime += Time.deltaTime;
        
        // Debug.Log(stepDelay);
        if (Time.time >= stepTime)
        {
            Step();
        }
        Board.Set(this);
    }

    private void OnMove(InputValue value)
    {
        Board.Clear(this);
        
        moveDir = value.Get<Vector2>();
        // Debug.Log(moveDir);
        //move left right down, cannot move up
        if (moveDir.y <1)
        {
            bool valid = Move(Vector2Int.RoundToInt(moveDir));
            Debug.Log(valid);
        }

        
        Board.Set(this);

    }

    
    //hard drop
    private void OnButton1()
    {
        Board.Clear(this);
        while (Move(Vector2Int.down)) //keep moving down until we can't anymore
        {
            continue;
        }
        Lock();
        Board.Set(this);
    }
    //rotate cw
    private void OnButton2()
    {
        Board.Clear(this);
        Rotate(-1);
        Board.Set(this);
    }

    //rotate ccw
    private void OnButton3()
    {
        Board.Clear(this);
        Rotate(1);
        Board.Set(this);
    }
    
    //remeber for each button to Clear board, add to locktime and Set board

    private void Step()
    {
        stepTime = Time.time + stepDelay;
        Move(Vector2Int.down);
        if (lockTime >= lockDelay)
        {
            Lock();
        }
    }

    //lock pieces in palce
    public void Lock()
    {
        Board.Set(this);
        Board.ClearLines();
        if (!Board.selfOver)
        {
            Board.SpawnPiece();
        }
        
        // if (isSpecial)
        // {
        //     Board.RecordSpecial();
        // }
    }

    
    private bool Move(Vector2Int translation)
    {
        Vector3Int newPosition = Position;
        newPosition.x += translation.x;
        newPosition.y += translation.y;
        //need to check if this is a valid position using game board
        bool valid = Board.IsValidPosition(this,newPosition);
        if (valid)
        {
            Position = newPosition;
            lockTime = 0f; //reset lock time on ovememt
        }
        

        return valid;
    }

    private void Rotate(int dir)
    {
        int originalrot = rotationInd;
        rotationInd = Wrap(rotationInd+dir, 0, 4);
        ApplyRotationMatrix(dir);
        if (!TestWallkicks(rotationInd, dir))//undo
        {
            rotationInd = originalrot;
            ApplyRotationMatrix(-dir);
            lockTime = 0f;
        }
        
    }
    private void ApplyRotationMatrix(int dir) //rotate using rotation matrix
    {
        for (int i = 0; i < Cells.Length; i++)
        {
            Vector3 cell = Cells[i];
            int x, y; //coordinates after rotates
            switch (data.tetromino)
            {
                //I and O rotated differently
                case Tetromino.I:
                case Tetromino.O:
                    //rotates around differen poiny
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x =  Mathf.CeilToInt(cell.x * Data.RotationMatrix[0] * dir + cell.y * Data.RotationMatrix[1] * dir);
                    y =  Mathf.CeilToInt(cell.x * Data.RotationMatrix[2] * dir + cell.y * Data.RotationMatrix[3] * dir);
                    break;
                default:
                    x = Mathf.RoundToInt(cell.x * Data.RotationMatrix[0] * dir + cell.y * Data.RotationMatrix[1] * dir);
                    y =  Mathf.RoundToInt (cell.x * Data.RotationMatrix[2] * dir + cell.y * Data.RotationMatrix[3] * dir);
                    break;
            }

            Cells[i] = new Vector3Int(x, y, 0);
        }

    }

    private bool TestWallkicks(int rotationIndex, int rotationDirection)
    {
        int wallkickInd = GetWallkickInd(rotationIndex, rotationDirection);
        for (int i = 0; i  < data.Wallkicks.GetLength(1); i++) //check tests
        {
            Vector2Int translation = data.Wallkicks[wallkickInd, i];
            if (Move(translation))
            {
                return true;
            }
        }

        return false;
    }
    
    private int GetWallkickInd(int rotationIndex, int rotationDirection)
    {
        int wallkickInd = rotationIndex * 2;
        if (rotationDirection < 0)
        {
            wallkickInd--;
        }

        return Wrap(wallkickInd, 0, data.Wallkicks.GetLength(0));
    }
    private int Wrap(int input, int min, int max)
    {
        if (input < min)
        {
            return max - (min - input) % (max - min);
        }
        else
        {
            return min + (input - min) % (max - min);
        }
    }
    
}
