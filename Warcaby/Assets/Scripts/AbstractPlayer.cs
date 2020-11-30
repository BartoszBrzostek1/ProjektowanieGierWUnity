using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Player : MonoBehaviour
{

    public abstract void Turn();

    public int Direction; //kierunek gracza


}
