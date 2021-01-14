using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour
{
    static private Controller instance = null;
    public int MarkedPlayerIndex = 0; //index gracza
    public Player[] players = new Player[2]; // tablica dwóch graczy
    static public readonly string PawnTag = "Pawn";
    static public readonly string FieldTag = "Field";


    public static Controller GetInstance() //singleton
    {
        if (instance == null)
        {
            instance = GameObject.FindGameObjectWithTag("GameController").GetComponent<Controller>();
        }

        return instance;
    }

    public Player MarkedPlayer //aktualny gracz
    {
        get
        {
            return players[MarkedPlayerIndex];
        }
    }

    public void Next() //zmiana na następny ruch
    {
        MarkedPlayerIndex = 1 - MarkedPlayerIndex;
        DataGameScript.GetInstance().UpdateListMoves();

        int winner = DataGameScript.GetInstance().Win();
        if(winner != -1)
        {
            ViewBoard.GetInstance().GameEndUI.SetActive(true);
            ViewBoard.GetInstance().GameEndUI.GetComponent<EndView>().SetEndText(winner);
        }
    }

    public void Initializet()
    {
        players[0] = GameObject.Find("Player1").GetComponent<Player>();
        players[0].Direction = 1; //ustawiamy obiekty i ich kierunek

        players[1] = GameObject.Find("Player2").GetComponent<Player>();
        players[1].Direction = -1;

        for (int y = 0; y < DataGameScript.GetInstance().Board_size; y++)
        {
            for (int x = 0; x < DataGameScript.GetInstance().Board_size; x++) //sprwdzamy wszystkie pola
            {
                if ((x % 2 == 0 && y % 2 == 0) || (x % 2 == 1 && y % 2 == 1)) //sprawdza parzystość pól 
                {
                    if (y < DataGameScript.GetInstance().BeginRows) //ustawienie pionków na górze
                    {
                        DataGameScript.GetInstance().board[x, y].Owner = players[0];
                    }
                    else if (y >= DataGameScript.GetInstance().Board_size - DataGameScript.GetInstance().BeginRows)
                    {
                        DataGameScript.GetInstance().board[x, y].Owner = players[1];
                    }
                }
            }
        }
        DataGameScript.GetInstance().UpdateListMoves();
    }


    void Update()
    {
        MarkedPlayer.Turn();
    }
}
