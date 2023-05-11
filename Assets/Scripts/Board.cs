using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // Start is called before the first frame update
    private Tilemap tilemap;
    private Dictionary<int, List<Vector3Int>> specialIndToPieces;
    private Dictionary<Vector3Int, int> pieceToSpecialInd;
    private int specialInd = 0;
    
    public float pSpecial = 0.2f;
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
        float numLinesCleared = 0f;
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
        }

    }

    public int[] LineClear(int row)
    {
        int[] specialTilesVisited = new int[0];
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

        return specialTilesVisited;
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

    public void RecordSpecial()
    {
        
        Debug.Log("special");
        specialIndToPieces[specialInd] = new List<Vector3Int>();
        for (int i = 0; i < activePiece.Cells.Length; i++)
        {
            Vector3Int pos = activePiece.Cells[i] + activePiece.Position;
            specialIndToPieces[specialInd].Add(pos);
            pieceToSpecialInd[pos] = specialInd;
            Debug.Log(specialIndToPieces[specialInd][i]);
        }
        

        specialInd++;
        
        
    }
}
