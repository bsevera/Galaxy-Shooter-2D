using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    private bool _isGameOver;
    private UIManager _UIManager;

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
            SceneManager.LoadScene("Game"); //scene 2 = game
        }

        if (Input.GetKeyDown(KeyCode.M) && _isGameOver)
        {
            SceneManager.LoadScene("MainMenu");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();            
        }

    }

    private void PauseGame()
    {
        _UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_UIManager != null)
        {
            _UIManager.PauseGame();
        }
        else
        {
            Debug.LogError("GameManager :: PauseGame :: UIManager is null");
        }

        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
    
    public void QuitGame()
    {
        LoadMainMenu();
    }

    private IEnumerator ReturnToMainMenuRoutine()
    {
        yield return new WaitForSeconds(10f);
        LoadMainMenu();
    }

    private void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
