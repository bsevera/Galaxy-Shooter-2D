using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering.PostProcessing;



public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _killsText;

    [SerializeField]
    private TMP_Text _waveText;

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

    [SerializeField]
    private TMP_Text _homingMissileText;
    
    private Player _player;

    [SerializeField]
    private Sprite[] _liveSprites;

    [SerializeField]
    private Slider _thrusterGauge;
       
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

    public void DisplayWaveText(int wave)
    {
        _waveText.text = "Wave " + wave;
        _waveText.gameObject.SetActive(true);

        StartCoroutine(HideWaveText());
    }

    public void UpdateKills(int _killed, int maxEnemiesInWave)
    {
        System.Text.StringBuilder kills = new System.Text.StringBuilder();
        kills.Append("Kills: ");
        kills.Append(_killed.ToString());
        kills.Append(" / ");
        kills.Append(maxEnemiesInWave);

        _killsText.text = kills.ToString();
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

    public void UpdateHomingMissileCount(int currentHomingMissileCount)
    {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append("= ");
        sb.Append(currentHomingMissileCount.ToString());

        _homingMissileText.text = sb.ToString();
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

    IEnumerator HideWaveText()
    {
        yield return new WaitForSeconds(3.0f);
        _waveText.gameObject.SetActive(false);
    }

    #region Thruster Gauge
    public void SetThrusterGaugeMax(float maxThrusterValue)
    {
        _thrusterGauge.maxValue = maxThrusterValue;        
    }

    public void UpdateThrusterGauge(float currentThrusterValue)
    {
        _thrusterGauge.value = currentThrusterValue;

        if (_thrusterGauge.value > 0)
        {
            _thrusterGauge.fillRect.gameObject.SetActive(true);
        }
        else
        {
            _thrusterGauge.fillRect.gameObject.SetActive(false);
        }
    }
    #endregion

}
