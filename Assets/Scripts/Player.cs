using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _speedMultiplier = 2.0f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private bool _isTripleShotActive = false;

    //[SerializeField]
    //private bool _isSpeedBoostActive = false;

    [SerializeField]
    private bool _IsShieldsActive = false;

    [SerializeField]
    private float _fireRate = 0.15f;

    //cool down system for limiting how quickly the player can fire
    private float _canFire = -1f;

    [SerializeField]
    private int _lives = 3;

    [SerializeField]
    private float _tripleShotActiveLengthOfTime = 5.0f;

    [SerializeField]
    private float _speedActiveLengthOfTime = 5.0f;

    [SerializeField]    
    private GameObject _shields;

    [SerializeField]
    private GameObject _leftEngine;

    [SerializeField]
    private GameObject _rightEngine;

    [SerializeField]
    private AudioClip _laserShotAudioClip;

    [SerializeField]
    private AudioClip _powerUpAudioClip;

    private AudioSource _AudioSource;

    private SpawnManager _spawnManager;

    [SerializeField]
    private int _score = 0;
    
    private UIManager _UIManager;

    // Start is called before the first frame update
    void Start()
    {

        //take the current position and assign it a start position of 0, 0, 0 
        transform.position = new Vector3(0, 0, 0);

        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("spawnManager is null!");
        }

        //Find UIManager and cache it for later
        _UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_UIManager == null)
        {
            Debug.Log("UIManager is null");
        }

        //get the audio component of the player and assign the audio clip for the fire laser sound
        _AudioSource = GetComponent<AudioSource>();
        if (_AudioSource == null)
        {
            Debug.LogError("Audio Source of the Player is Null");
        }
    }

    // Update is called once per frame
    void Update()
    {

        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

    }

    private void PlayPowerUpSoundEffect()
    {
        _AudioSource.clip = _powerUpAudioClip;
        _AudioSource.Play();
    }

    public void TripleShotActive()
    {
        PlayPowerUpSoundEffect();
        _isTripleShotActive = true;
        StartCoroutine(PowerDownTripleShot());
    }

    public void SpeedBoostActive()
    {
        PlayPowerUpSoundEffect();
        _speed *= _speedMultiplier;        
        StartCoroutine(PowerDownSpeedBoost());        
    }

    public void ShieldsActive()
    {
        PlayPowerUpSoundEffect();
        _IsShieldsActive = true;
        _shields.SetActive(true);
    }

    public void IncreaseScore(int incomingPoints)
    {
        _score += incomingPoints;
        if (_UIManager != null)
        {
            _UIManager.UpdateScore(_score);
        }
    }

    public void Damage()
    {
        if (_IsShieldsActive)
        {
            //deactivate shields
            _IsShieldsActive = false;
            _shields.SetActive(false);
            return;
        }
        

        _lives -= 1;

        //update lives count in UI
        _UIManager.UpdateLives(_lives);

        switch (_lives)
        {
            case 2:
                _rightEngine.SetActive(true);
                break;
            case 1:
                _leftEngine.SetActive(true);
                break;
            case 0:
                PlayerDied();
                break;
        }
            

        //check if dead, if so, destroy us
        //if (_lives < 1)
        //{            
        //    //get spawn_manager object and tell it the player is dead to stop enemies from spawning
            
        //    if (_spawnManager != null)
        //    {
        //        _spawnManager.OnPlayerDeath();
        //    }
        //    else
        //    {
        //        Debug.Log("spawnmanager = null");
        //    }

        //    //destroy the player
        //    Destroy(this.gameObject);
        //}
    }

    void PlayerDied()
    {
        if (_spawnManager != null)
        {
            _spawnManager.OnPlayerDeath();
        }
        else
        {
            Debug.Log("spawnmanager = null");
        }

        //destroy the player
        Destroy(this.gameObject);

    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }
        else
        {
            //laser needs to be spawned .8f above the player object
            Vector3 laserStartingPosition = new Vector3(transform.position.x, transform.position.y + 1.05f, 0);
            Instantiate(_laserPrefab, laserStartingPosition, Quaternion.identity);
        }

        //play the audio clip
        _AudioSource.clip = _laserShotAudioClip;
        _AudioSource.Play();
    }

    //void SetSpeed()
    //{
    //    if (Input.GetKeyDown(KeyCode.Equals)) //plus key is located on the equals key
    //    {
    //        if (_speed < 11.0f)
    //        {
    //            _speed += 1.0f;                
    //        }
    //    }
    //    else if (Input.GetKeyDown(KeyCode.Minus))
    //    {
    //        if (_speed > 0.0f)
    //        {
    //            _speed -= 1.0f;                
    //        }
    //    }
    //}

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        //constrain the object to not move past 8 and -8 on the horizontal
        //constrain the object to not move past 0 and -3.5 on the vertical
        if (transform.position.x >= 8)
        {
            transform.position = new Vector3(8, transform.position.y, 0);
        }
        else if (transform.position.x <= -8)
        {
            transform.position = new Vector3(-8, transform.position.y, 0);
        }

        if (transform.position.y >= 0)
        {
            transform.position = new Vector3(transform.position.x, 0, 0);
        }
        else if (transform.position.y <= -3.5f)
        {
            transform.position = new Vector3(transform.position.x, -3.5f, 0);
        }

    }

    IEnumerator PowerDownTripleShot()
    {
        yield return new WaitForSeconds(_tripleShotActiveLengthOfTime);
       
        _isTripleShotActive = false;                        

    }

    IEnumerator PowerDownSpeedBoost()
    {
        yield return new WaitForSeconds(_speedActiveLengthOfTime);
        //_isSpeedBoostActive = false;
        _speed /= _speedMultiplier;
    }
}
