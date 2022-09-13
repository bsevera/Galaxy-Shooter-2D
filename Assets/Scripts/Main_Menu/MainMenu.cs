using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene(1); //scene 1 = game
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
