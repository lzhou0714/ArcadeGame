using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameEndManager : MonoBehaviour
{
    [SerializeField] GameObject boardObject;
    [SerializeField] TextMeshProUGUI winnerText, scoreText;
    [SerializeField] Board[] board;
    static GameEndManager self;
    [SerializeField] GameObject inGameScoreObjs, pressAnyKeyTxtObj;

    int gameEndTimer;

    public enum Players
    {
        Player1, Player2,
    }
    private void Awake()
    {
        self = GetComponent<GameEndManager>();
    }

    void Start()
    {
        //EndGame(Players.Player2);
    }

    public static void EndGame(Players winningPlayer)
    {
        self.boardObject.SetActive(false);
        self.inGameScoreObjs.SetActive(false);

        if (winningPlayer == Players.Player1)
        {
            self.winnerText.text = "Player 1 Wins!";
        }
        else
        {
            self.winnerText.text = "Player 2 Wins!";
        }

        self.scoreText.text = "Player 1 Score: " + self.board[0].GetScore() + "\n\nPlayer 2 Score: " + self.board[1].GetScore();

        if (self.gameEndTimer < 1)
        {
            self.gameEndTimer = 1;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (gameEndTimer > 0)
        {
            if (gameEndTimer > 50)
            {
                if (gameEndTimer % 3 == 0 && gameEndTimer < 78)
                {
                    pressAnyKeyTxtObj.SetActive(!pressAnyKeyTxtObj.activeSelf);
                }
                if (Input.anyKey)
                {
                    SceneManager.LoadScene("StartScene");
                }
            }
            gameEndTimer++;
        }
    }
}
