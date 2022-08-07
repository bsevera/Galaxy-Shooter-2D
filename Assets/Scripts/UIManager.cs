using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;

    [SerializeField]
    private TMP_Text _gameOverText;

    [SerializeField]
    private TMP_Text _restartText;

    [SerializeField]
    private Image _livesCountImage;
    
    private Player _player;

    [SerializeField]
    private Sprite[] _liveSprites;
    
    private GameManager _gameManager;
    

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (_gameManager == null)
        {
            Debug.LogError("_gameManager is Null");
        }

        //Ensure game over text is not displaying
        _gameOverText.gameObject.SetActive(false);

        //set score to 0
        _scoreText.text = "Score: 0";

        //assign text component to the handle
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.Log("Player object is null in UIManager");
        }

    }


    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateLives(int currentLives)
    {
        _livesCountImage.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        
        _gameManager.GameOver();

        StartCoroutine(GameOverFlickeringRoutine());
    }

    IEnumerator GameOverFlickeringRoutine()
    {
        while (true)
        {
            if (_gameOverText.IsActive())
            {
                _gameOverText.gameObject.SetActive(false);
            }
            else
            {
                _gameOverText.gameObject.SetActive(true);
            }
            yield return new WaitForSeconds(0.5f);
        }        
    }
    
}
