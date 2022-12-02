using UnityEngine;
using UnityEngine.SceneManagement;

public class Scoring : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
