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

    public void ReturnToMainMenu()
    {
        StartCoroutine(ReturnToMainMenuRoutine());
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && _isGameOver)
        {
            //reload the scene            
            SceneManager.LoadScene(2); //scene 2 = game
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

    }

    private IEnumerator ReturnToMainMenuRoutine()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene("MainMenu");
    }
}
