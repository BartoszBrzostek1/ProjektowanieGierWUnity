using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBoard : MonoBehaviour
{

    public GameObject FieldPrefab;
    public GameObject PawnPrefab;
    //public Color PlayerWhite = Color.white;
    //public Color PlayerBlack = Color.grey;
    //public Color Queen1 = Color.yellow;
    //public Color Queen2 = Color.green;
    //public Color LightPawnColor = Color.red;
    private static ViewBoard instance;
    public GameObject GameEndUI;
    public GameObject InGameUI;

    public static ViewBoard GetInstance()
    {
        if (instance == null)
        {
            instance = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ViewBoard>();
        }
        return instance;
    }

    private Vector3 GetFieldPosition(int x, int y, bool IsPawn)
    {
        MeshRenderer renderer = FieldPrefab.GetComponent<MeshRenderer>(); //bierzemy obiekt
        float fieldSize = renderer.bounds.size.x * FieldPrefab.transform.localScale.x; //oblicza rozmiar pola przez lokaną skale
        float startFieldPosition = 0.0f - (fieldSize * DataGameScript.GetInstance().Board_size / 2); //wyliczamy początkowe pole
        return new Vector3(startFieldPosition + x * fieldSize, startFieldPosition + y * fieldSize, (IsPawn ? -0.1f : 0.0f)); //zwracamy trójwymiarowy wektor
    }

    public void LightPawn(DataPawn toLight, DataPawn preLight = null)
    {
        if (preLight)
        {
            preLight.gameObject.GetComponent<MeshRenderer>().material.color = (preLight.Owner == Controller.GetInstance().players[0] ? Color.white : Color.grey); //ustawienie kolorów pionków przed kliknięciem
        }
        if (toLight)
        {
            toLight.gameObject.GetComponent<MeshRenderer>().material.color = Color.red; //ustawienie kolorów pionków po kliknięciu
        }
    }

    public void DeletePawn (DataPawn pawn) //niszczenie pionka
    {
        Destroy(pawn.gameObject);
    }

    public void MovePawn(DataPawn pawn, Vector2 target ) //przesuwanie pionka
    {
        pawn.gameObject.transform.position = GetFieldPosition(target.x, target.y, true);
    }

    public void PromotePawn(DataPawn pawn) //zmiana wyglądu awansowanego pionka
    {
        pawn.transform.localScale = new Vector3(1.15f * pawn.transform.localScale.x, pawn.transform.localScale.y, 1.15f * pawn.transform.localScale.z);
        pawn.GetComponent<MeshRenderer>().material.color = (pawn.Owner == Controller.GetInstance().players[0] ? new Color(22f, 22f, 22f, 1) : new Color(88f, 88f, 88f, 1));
    }

    public void Initializet()
    {
        for (int y = 0; y < DataGameScript.GetInstance().Board_size; y++)
        {
            for (int x = 0; x < DataGameScript.GetInstance().Board_size; x++) 
            {
                MeshRenderer newField = (Instantiate(FieldPrefab, GetFieldPosition(x, y, false), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject).GetComponent<MeshRenderer>();
                newField.GetComponent<DataField>().X = x;
                newField.GetComponent<DataField>().Y = y;
                if ((x % 2 == 0 && y % 2 == 0) || (x % 2 == 1 && y % 2 == 1)) //warunek na parzyste pola || warunek na nieparzyste
                {
                    newField.material.color = Color.black;
                    if (y < DataGameScript.GetInstance().BeginRows)
                    {
                        GameObject pawn = Instantiate(PawnPrefab, GetFieldPosition(x, y, true), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
                        pawn.GetComponent<MeshRenderer>().material.color = Color.white;
                        DataGameScript.GetInstance().RegisterPawnPosition(pawn.GetComponent<DataPawn>(), new Vector2(x, y));

                    }
                    else if (y >= DataGameScript.GetInstance().Board_size - DataGameScript.GetInstance().BeginRows)
                    {
                        GameObject pawn = Instantiate(PawnPrefab, GetFieldPosition(x, y, true), Quaternion.Euler(90.0f, 0.0f, 0.0f)) as GameObject;
                        pawn.GetComponent<MeshRenderer>().material.color = Color.grey;
                        DataGameScript.GetInstance().RegisterPawnPosition(pawn.GetComponent<DataPawn>(), new Vector2(x, y));
                    }
                }
                //wypełniamy plansze
            }
        }
    }

}
