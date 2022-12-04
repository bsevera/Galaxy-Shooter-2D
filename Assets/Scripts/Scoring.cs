using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class Scoring : MonoBehaviour
{
    [SerializeField]
    private GameObject _scoringPanel;

    [SerializeField]
    private GameObject _powerupsPanel;

    [SerializeField]
    private TMP_Text _prevNextButtonText;

    private int _currentPage;

    private void Awake()
    {
        _currentPage = 1;
        _prevNextButtonText.text = "Powerups";
        _scoringPanel.SetActive(true);
        _powerupsPanel.SetActive(false);
    }
    public void ReturnToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void PrevNextButton()
    {
        if (_currentPage == 1)
        {
            _currentPage = 2;
            _prevNextButtonText.text = "Scoring";
            _scoringPanel.SetActive(false);
            _powerupsPanel.SetActive(true);
        }
        else
        {
            _currentPage = 1;
            _prevNextButtonText.text = "Powerups";
            _scoringPanel.SetActive(true);
            _powerupsPanel.SetActive(false);
        }
    }
}
