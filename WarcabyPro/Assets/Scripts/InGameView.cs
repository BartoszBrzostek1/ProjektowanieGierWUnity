using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameView : MonoBehaviour
{
    // Start is called before the first frame update
    public void ExitToMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(0); //ładujemy ponownie scene
    }
}
