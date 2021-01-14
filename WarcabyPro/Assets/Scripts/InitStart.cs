using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InitStart : MonoBehaviour
{

    public GameObject DataGameScript;
    public GameObject GameController;
    public GameObject Player;
    public StartView StartView;

    public void StartClassic()
    {
        GameObject gameData = Instantiate(DataGameScript);
        gameData.GetComponent<DataGameScript>().Board_size = 8;
        gameData.GetComponent<DataGameScript>().BeginRows = 3;
        gameData.GetComponent<DataGameScript>().Initializet();

        GameObject player1 = Instantiate(Player);
        player1.name = "Player1";
        GameObject player2 = Instantiate(Player);
        player2.name = "Player2";

        Camera.main.GetComponent<ViewBoard>().Initializet();
        Instantiate(GameController).GetComponent<Controller>().Initializet();

        Destroy(StartView.gameObject);
        ViewBoard.GetInstance().InGameUI.SetActive(true);
    }

    public void StartInternational()
    {
        GameObject gameData = Instantiate(DataGameScript);
        gameData.GetComponent<DataGameScript>().Board_size = 10;
        gameData.GetComponent<DataGameScript>().BeginRows = 4;
        gameData.GetComponent<DataGameScript>().Initializet();

        GameObject player1 = Instantiate(Player);
        player1.name = "Player1";
        GameObject player2 = Instantiate(Player);
        player2.name = "Player2";

        Camera.main.GetComponent<ViewBoard>().Initializet();
        Instantiate(GameController).GetComponent<Controller>().Initializet();

        Destroy(StartView.gameObject);
        ViewBoard.GetInstance().InGameUI.SetActive(true);
    }

    public void StartTest()
    {

        GameObject gameData = Instantiate(DataGameScript);
        gameData.GetComponent<DataGameScript>().Board_size = 6;
        gameData.GetComponent<DataGameScript>().BeginRows = 1;
        gameData.GetComponent<DataGameScript>().Initializet();

        GameObject player1 = Instantiate(Player);
        player1.name = "Player1";
        GameObject player2 = Instantiate(Player);
        player2.name = "Player2";

        Camera.main.GetComponent<ViewBoard>().Initializet();
        Instantiate(GameController).GetComponent<Controller>().Initializet();

        Destroy(StartView.gameObject);
        ViewBoard.GetInstance().InGameUI.SetActive(true);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
        

}
