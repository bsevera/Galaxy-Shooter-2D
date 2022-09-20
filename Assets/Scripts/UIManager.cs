using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;



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

    [SerializeField]
    private TMP_Text _ammoText;
    
    private Player _player;

    [SerializeField]
    private Sprite[] _liveSprites;
       
    private GameManager _gameManager;
    
    private PostProcessVolume _postProcessVolume;
    private LensDistortion _lensDistortion;

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
        
        ApplyLensDistortion();
    }


    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore;
    }

    public void UpdateAmmo(int currentAmmo, int maxAmmo)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("Ammo: ");
        sb.Append(currentAmmo.ToString());
        sb.Append("/");
        sb.Append(maxAmmo.ToString());

        _ammoText.text = sb.ToString();
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

    private void ApplyLensDistortion()
    {
        _postProcessVolume = GameObject.Find("Post Process Volume").GetComponent<PostProcessVolume>();
        if( _postProcessVolume != null )
        {
            _postProcessVolume.profile.TryGetSettings(out _lensDistortion);

            StartCoroutine(ApplyLensDistortionEffect());
        }
        else
        {
            Debug.LogError("Post Process Volume is NULL");
        }
    }

    IEnumerator ApplyLensDistortionEffect()
    {
        int val = -100;

        _lensDistortion.intensity.value = val;
        while (_lensDistortion.intensity.value < 0)
        {
            val += 1;
            _lensDistortion.intensity.value = val; 
            
            yield return new WaitForSeconds(0.05f);
        }
    }

}
