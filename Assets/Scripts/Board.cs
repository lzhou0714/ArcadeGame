using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using TMPro;
using Unity.Mathematics;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Quaternion = UnityEngine.Quaternion;

public class Board : MonoBehaviour
{
    // Start is called before the first frame update
    private Tilemap tilemap;

    public float numLinesCleared;
    //for later////
    private Dictionary<int, List<Vector3Int>> specialIndToPieces;
    private Dictionary<Vector3Int, int> pieceToSpecialInd;
    private int specialInd = 0;
    public float pSpecial = 1f;
    //for later////

    public float doubleRateTimer; //timer for doubled fall rate
    public float halfRateTimer; //timer for half rate
    private bool doubleRate;
    private bool halfRate;
    public bool isplayer1;
    public bool isplayer2;
    [SerializeField] TMP_Text gameOverText;

    public bool selfOver;
    

    [SerializeField] private GameObject inkSplatter;

    [SerializeField] private Board opponent;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text highScoreText;
    private int totalScore  = 0;
    private int highScore;
    public TData[] tetrominos;
    public Piece activePiece { get; private set; }
    public Vector3Int spawnPosition;
    public Vector2Int boardSize = new Vector2Int(10, 20);
    
    public RectInt Bounds
    {
        get
        {
            //position by default in center 
            Vector2Int position = new Vector2Int((int) -boardSize.x/2, 
                - boardSize.y/2);
            return new RectInt(position, boardSize);
        }
    }
    private void Awake()
    {
        gameOverText.enabled = false;
        specialIndToPieces = new Dictionary<int, List<Vector3Int>>();
        pieceToSpecialInd = new Dictionary<Vector3Int, int>();
        tilemap = GetComponentInChildren<Tilemap>();
        activePiece = GetComponentInChildren<Piece>();
        
        //loop through data and initialize teterominos data
        for (int i = 0; i < tetrominos.Length; i++)
        {
            tetrominos[i].Initialize();
        }
    }

    private void Start()
    {
        SpawnPiece();
    }

    private void Update()
    {
        highScore = PlayerPrefs.GetInt("High Score", 0);
        highScoreText.text = "High Score: " + highScore.ToString();
        scoreText.text = "Score: " + totalScore.ToString();
        
        if (halfRateTimer > 0)
        {
            halfRateTimer -= Time.deltaTime;
            if (halfRateTimer < 0)
            {
                halfRateTimer = 0f;
                activePiece.stepDelay /= 2; //return to normal
                Debug.Log("half rate over: " + activePiece.stepDelay);
            }
        }

        if (doubleRateTimer > 0)
        {
            doubleRateTimer -= Time.deltaTime;
            if (doubleRateTimer < 0)
            {
                doubleRateTimer = 0f;
                activePiece.stepDelay *= 2; //return to normal
                Debug.Log("double rate over: " + activePiece.stepDelay);

            }
        }
    }

    public void SpawnPiece()
    {
        int random = Random.Range(0, tetrominos.Length);
        TData data = tetrominos[random];
        
        activePiece.Initialize(this,spawnPosition , data);
        if (IsValidPosition(activePiece, spawnPosition))
        {
            Set(activePiece);
        }
        else
        {
            
            GameOver();
        }
    }

    private void GameOver()
    {
        tilemap.ClearAllTiles();
        selfOver = true;
        gameOverText.enabled = true;
        scoreText.enabled = false;
        
        if (opponent.selfOver)
        {
            gameOverText.enabled = false;
            if (opponent.isplayer1)
                GameEndManager.EndGame(GameEndManager.Players.Player1);
            else
            {
                GameEndManager.EndGame(GameEndManager.Players.Player2);
            }
        }
        
        
    }

    public void Set(Piece piece)
    {
        for (int i=0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            tilemap.SetTile(tilePosition, piece.data.tile);
        }
    }

    public void Clear(Piece piece)
    {
        
        for (int i=0; i < piece.Cells.Length; i++)
        {
            Vector3Int tilePosition = piece.Cells[i] + piece.Position;
            tilemap.SetTile(tilePosition, null);
        }
    }
    
    
    public bool IsValidPosition(Piece piece, Vector3Int position)
    {
        RectInt bounds = Bounds;
        //need to check each cell in piece is valid
        for (int i = 0; i < piece.Cells.Length; i++)
        {
            //position of each cell in piece is given by the piece's position
            // plus the offset of the tile given by piece.cells
            Vector3Int tilePosition = piece.Cells[i] + position;
            //checks out of bounds
            if (!bounds.Contains((Vector2Int)tilePosition))
            {
                return false;
            }
            
            if (tilemap.HasTile(tilePosition)) //overlapping tiles
            {
                return false;
            }
            
        }

        return true;
    }

    
    //clears line that are full
    public void ClearLines()
    {
        // numLinesCleared = 0f;
        RectInt bounds = Bounds;
        int row = Bounds.yMin;
        
        while (row < Bounds.yMax)
        {
            if (isLineFull(row))
            {
                numLinesCleared++;
                LineClear(row);
                //don't iterate to next row when line is full bc the above rows will fall down
            }
            else
            {
                row++;
            }
        }

        if (numLinesCleared > 0f)
        {
            
            totalScore += (int) Mathf.Pow(2f, numLinesCleared);
            if (totalScore > highScore)
            {
                PlayerPrefs.SetInt("High Score", totalScore);
                PlayerPrefs.Save();
            }

            switch (numLinesCleared)
            {
                case 1: //nothing happens
                    break;
                case 2: //half your fall rate
                    if (halfRateTimer == 0)
                    {
                        activePiece.stepDelay *= 2f; //half rate makes fall slower, retur to original speed
                        halfRateTimer = 20f;
                        Debug.Log(activePiece.stepDelay);
                    }

                    break;
                case 3: //double other player's fall rate
                    if (opponent.doubleRateTimer == 0)
                    {
                        opponent.activePiece.stepDelay /= 2f;
                        opponent.doubleRateTimer = 20f;
                    }
                    break;
                case 4: // splatter ink
                    Instantiate(inkSplatter, opponent.transform.position, Quaternion.identity);
                    break;
                default: //clear your entire board
                    tilemap.ClearAllTiles();
                    break;
            }
        }
        

    }
    

    public void LineClear(int row)
    {
        // int[] specialTilesVisited = new int[0];
        RectInt bounds = Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            
            this.tilemap.SetTile(pos,null);
        }

        while (row < Bounds.yMax)
        {
            for (int col = bounds.xMin; col < bounds.xMax; col++)
            {
                Vector3Int pos = new Vector3Int(col, row+1, 0); //get row above
                TileBase above = tilemap.GetTile(pos);
                pos = new Vector3Int(col, row, 0);
                tilemap.SetTile(pos, above);
            }

            row++;
        }

    }
    
    //checks if line is full and need to be cleared
    public bool isLineFull(int row)
    {
        RectInt bounds = Bounds;
        for (int col = bounds.xMin; col < bounds.xMax; col++)
        {
            Vector3Int pos = new Vector3Int(col, row, 0);
            if (!this.tilemap.HasTile(pos))
            {
                return false;
            }
        }

        return true;
    }

    public int GetScore()
    {
        return totalScore;
    }
    
}
