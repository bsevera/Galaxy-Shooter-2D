using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void LoadGame()
    {
        SceneManager.LoadScene(2); //scene 2 = game
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
