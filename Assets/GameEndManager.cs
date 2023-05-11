using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameEndManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshPro;
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

    void NewFunction()
    {
        //if (Input.GetKey(Attack))
        if (AttackPressed())
        {

        }

    }

    bool AttackPressed()
    {
        return Input.GetKey(Attack) || Input.GetKey(AttackAlternate);
    }

    KeyCode Attack = KeyCode.K;
    KeyCode AttackAlternate = KeyCode.U;
    // Update is called once per frame
    void Update()
    {
        
    }
}
