using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Credits : MonoBehaviour
{
    // Start is called before the first frame update
    public void Start()
    {
        StartCoroutine(ReturnToMainMenu());
    }

    IEnumerator ReturnToMainMenu()
    {
        yield return new WaitForSeconds(19f);
        SceneManager.LoadScene("MainMenu");
    }
}
