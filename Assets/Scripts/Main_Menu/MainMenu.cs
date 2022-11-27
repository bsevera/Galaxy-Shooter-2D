using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene("Game"); //scene 2 = game
    }

    public void LoadCredits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
