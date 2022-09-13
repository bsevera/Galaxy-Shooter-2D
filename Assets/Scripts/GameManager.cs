using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _isGameOver;

    public void GameOver()
    {
        _isGameOver = true;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
        {
            //reload the scene            
            SceneManager.LoadScene(1); //scene 1 = game
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }
}
