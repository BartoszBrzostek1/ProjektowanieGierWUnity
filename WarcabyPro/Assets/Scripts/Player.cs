using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{

    public abstract void Turn(); //obsługuje sterowanie

    public int Direction; //kierunek gracza


}