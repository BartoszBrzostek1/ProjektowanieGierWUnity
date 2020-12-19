using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class HumanPlayer : Player
{
    private DataPawn markedPawn;
    public DataPawn MarkedPawn
    {
        get
        {
            return markedPawn;
        }
        set
        {
            ViewBoard.GetInstance().LightPawn(value, markedPawn);
            markedPawn = value;
        }
    }

    public override void Turn() //inizjalizacja logiki poruszania pionkami
    {
        if (Input.GetMouseButtonDown(0)) //jeśli naciśniesz prawy przycisk myszy
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // tworzymy promień od kierunku w którym patrzy kamera
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 50.0f)) //klikanie w obiekt jeśli go trafiliśmy
            {
                 if (markedState == PlayerState.SelectingPawn) //wybieramy pionek
                 {
                     if (hit.collider.tag == Controller.PawnTag) //zaznaczamy pionek
                     {
                         DataPawn pawn = hit.collider.GetComponent<DataPawn>();
                         if (Controller.GetInstance().MarkedPlayer == pawn.Owner) //jeśli gracz zaznaczył własny pionek
                         {
                             MarkedPawn = pawn;
                             markedState = PlayerState.SelectingField; //gdy zaznaczymy własny pionek zmieniamy na stan, w którym możemy wybrać ruch
                         }
                         else //jeśli pionek nie był nasz
                         {
                             MarkedPawn = null;
                         }
                     }
                     else if (hit.collider.tag == Controller.FieldTag) //jeśli naciśniemy na pole
                     {
                         MarkedPawn = null; //to po prostu odznaczamy pionka
                     }
                 } //klikneliśmy w pionka
                 else if (markedState == PlayerState.SelectingField)  //klikneliśmy w pole
                 {

                 }
            }
            else //jeśli nie trafimy to odznaczamy obiekt jeśli to możliwe
            {
                MarkedPawn = null;
                markedState = PlayerState.SelectingPawn; //stan zaminia się w proces wybierania pionka
            }
        }
    }



    enum PlayerState
    {
        SelectingPawn, SelectingField
    }

    private PlayerState markedState = PlayerState.SelectingPawn;


    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
