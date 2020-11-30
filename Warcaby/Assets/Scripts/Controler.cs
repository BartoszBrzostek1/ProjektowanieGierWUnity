using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controler : MonoBehaviour
{
    static private Controler instance;
    public int MarkedPlayerIndex = 0; //index gracza
    public Player[] players = new Player[2]; // tablica dwóch graczy

    static Controler GetInstance()
    {
        if(instance==null)
        {
            instance = GameObject.FindGameObjectWithTag("Controler").GetComponent<Controler>();
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


    // Start is called before the first frame update
    void Start()
    {
        players[0] = GameObject.Find("Player1").GetComponent<Player>();
        players[0].Direction = 1; //ustawiamy obiekty i ich kierunek

        players[1] = GameObject.Find("Player2").GetComponent<Player>();
        players[1].Direction = -1;

        for(int y = 0; y < DataGameScript.GetInstance().Board_size; y++)
        {
            for (int x = 0; x < DataGameScript.GetInstance().Board_size; x++) //sprwdzamy wszystkie pola
            {
                if ((x % 2 == 0 && y % 2 == 0) || (x % 2 == 1 && y % 2 ==1)) //sprawdza parzystość pól 
                {
                    if (y < DataGameScript.GetInstance().BeginRows) //ustawienie pionków na górze
                    {
                        DataGameScript.GetInstance().Board[x, y].Owner = players[0];
                    }
                    else if (y >= DataGameScript.GetInstance().Board_size - DataGameScript.GetInstance().BeginRows)
                    {
                        DataGameScript.GetInstance().Board[x, y].Owner = players[1];
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
