using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private GameObject _quitButton;

    public void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            _quitButton.SetActive(false);
        }

    }
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
