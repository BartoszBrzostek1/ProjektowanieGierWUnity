using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBoard : MonoBehaviour
{

    public GameObject FieldPrefab;
    public GameObject PawnPrefab;
    public Color PlayerWhite = Color.white;
    public Color PlayerGrey = Color.grey;
    private static ViewBoard instance;

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
            preLight.gameObject.GetComponent<MeshRenderer>().material.color = (preLight.Owner == Controller.GetInstance().players[0] ? Color.white : Color.black); //ustawienie kolorów pionków przed kliknięciem
        }
        if (toLight)
        {
            toLight.gameObject.GetComponent<MeshRenderer>().material.color = Color.blue; //ustawienie kolorów pionków po kliknięciu
        }
    }

    void Start()
    {
        for (int y = 0; y < DataGameScript.GetInstance().Board_size; y++)
        {
            for (int x = 0; x < DataGameScript.GetInstance().Board_size; x++) 
            {
                MeshRenderer newField = (Instantiate(FieldPrefab, GetFieldPosition(x, y, false), Quaternion.identity) as GameObject).GetComponent<MeshRenderer>();
                newField.GetComponent<DataField>().X = x;
                newField.GetComponent<DataField>().Y = y;
                if ((x % 2 == 0 && y % 2 == 0) || (x % 2 == 1 && y % 2 == 1)) //warunek na parzyste pola || warunek na nieparzyste
                {
                    newField.material.color = Color.black;
                    if (y < DataGameScript.GetInstance().BeginRows)
                    {
                        GameObject Pawn = Instantiate(PawnPrefab, GetFieldPosition(x, y, true), Quaternion.identity) as GameObject;
                        Pawn.GetComponent<MeshRenderer>().material.color = PlayerWhite;
                        DataGameScript.GetInstance().RegisterPawnPosition(Pawn.GetComponent<DataPawn>(), new Vector2(x, y));

                    }
                    else if (y >= DataGameScript.GetInstance().Board_size - DataGameScript.GetInstance().BeginRows)
                    {
                        GameObject Pawn = Instantiate(PawnPrefab, GetFieldPosition(x, y, true), Quaternion.identity) as GameObject;
                        Pawn.GetComponent<MeshRenderer>().material.color = PlayerGrey;
                        DataGameScript.GetInstance().RegisterPawnPosition(Pawn.GetComponent<DataPawn>(), new Vector2(x, y));
                    }
                }
                //wypełniamy plansze
            }
        }
    }

    void Update()
    {

    }
}
