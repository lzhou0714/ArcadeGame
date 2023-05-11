using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameEndManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshPro;
    [SerializeField] Board[] board;
    static GameEndManager self;

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
        EndGame(Players.Player2);
    }

    public static void EndGame(Players winningPlayer)
    {
        if (winningPlayer == Players.Player1)
        {
            self.textMeshPro.text = "Player 1 Wins!";
        }
        else
        {
            self.textMeshPro.text = "Player 2 Wins!";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
