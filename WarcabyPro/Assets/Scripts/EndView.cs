using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndView : MonoBehaviour
{
    public Text TextEnd;

    public void SetEndText(int winner)
    {
        if (winner == 1)
        {
            TextEnd.text = "Wygrał gracz biały";
        }
        else if (winner == 0)
        {
            TextEnd.text = "Wygrał gracz czarny";
        }
        else
        {
            TextEnd.text = "Remis";
        }
    }

}
