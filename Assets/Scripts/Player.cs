using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _thrusterMultiplier = 1.5f;

    [SerializeField]
    private float _speedMultiplier = 2.0f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private bool _isTripleShotActive = false;

    [SerializeField]
    private bool _IsShieldsActive = false;
    private int _shieldHealth = 0;
    private int _shieldHealthMax = 3;

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
    private GameObject _thrusters;
    private bool _speedBoostIsActive = false;

    [SerializeField]
    private GameObject _leftEngine;

    [SerializeField]
    private GameObject _rightEngine;

    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private AudioClip _explosionClip;

    [SerializeField]
    private AudioClip _laserShotAudioClip;

    [SerializeField]
    private AudioClip _powerUpAudioClip;

    [SerializeField]
    private AudioClip _shieldsDownClip;

    [SerializeField]
    private AudioClip _noAmmoClip;

    private AudioSource _AudioSource;

    private SpawnManager _spawnManager;

    [SerializeField]
    private int _score = 0;

    [SerializeField]
    private int _maxAmmo = 30;
    private int _currentAmmo;
    
    private UIManager _UIManager;
    //private Animator _animator;

    private bool _playerExploding = false;

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

        _currentAmmo = _maxAmmo;
        UpdateAmmoUI();

    }

    // Update is called once per frame
    void Update()
    {
        if (_playerExploding == false)
        {
            CalculateMovement();

            if (_currentAmmo > 0 && Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            {
                FireLaser();
            }
            else if (_currentAmmo == 0 && Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
            {
                _AudioSource.clip = _noAmmoClip;
                _AudioSource.Play();
            }
        }

    }

    private void PlayPowerUpSoundEffect()
    {
        _AudioSource.clip = _powerUpAudioClip;
        _AudioSource.Play();
    }
    
    public void AmmoCollected()
    {
        PlayPowerUpSoundEffect();
        _currentAmmo = _maxAmmo;
        UpdateAmmoUI();
    }

    public void ExtraLifeCollected()
    {
        PlayPowerUpSoundEffect();

        if (_lives < 3)
        {
            _lives += 1;
            _UIManager.UpdateLives(_lives);
            RemoveDamage();
        }
    }

    private void RemoveDamage()
    {
        switch (_lives)
        {
            case 3:
                _rightEngine.SetActive(false);
                break;
            case 2:
                _leftEngine.SetActive(false);
                break;
        }
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
        ShowThrusterImage(true);
        _speedBoostIsActive = true;
        _speed *= _speedMultiplier;        
        StartCoroutine(PowerDownSpeedBoost());        
    }

    //public void ShieldsActive()
    //{
    //    PlayPowerUpSoundEffect();        
    //    _IsShieldsActive = true;
    //    _shields.SetActive(true);
    //}
    public void ShieldsActive()
    {
        PlayPowerUpSoundEffect();

        if (_IsShieldsActive)
        {
            if (_shieldHealth < _shieldHealthMax)
            {
                _shieldHealth += 1;
            }
        }
        else
        {
            _shieldHealth = _shieldHealthMax;
            _IsShieldsActive = true;
            _shields.SetActive(true);
        }

        SetShieldImage();
    }


    private void SetShieldImage()
    {
        switch (_shieldHealth)
        {
            case 3:
                _shields.transform.localScale = new Vector3(2.1f, 2.1f, 2.1f);
                break;
            case 2:
                _shields.transform.localScale = new Vector3(1.4f, 1.4f, 1.4f);
                break;
            case 1:
                _shields.transform.localScale = new Vector3(.7f, .7f, .7f);
                break;
        }
    }

    public void IncreaseScore(int incomingPoints)
    {
        _score += incomingPoints;
        if (_UIManager != null)
        {
            _UIManager.UpdateScore(_score);
        }
    }

    private void UpdateAmmoUI()
    {
        if (_UIManager != null)
        {
            _UIManager.UpdateAmmo(_currentAmmo, _maxAmmo);
        }
    }

    //public void Damage()
    //{
    //    if (_IsShieldsActive)
    //    {
    //        //deactivate shields
    //        _IsShieldsActive = false;
    //        _shields.SetActive(false);
    //        return;
    //    }

    //    //somehow player lives was getting below 0
    //    if (_lives > 0)
    //    {
    //        _lives -= 1;
    //    }

    //    //update lives count in UI
    //    _UIManager.UpdateLives(_lives);

    //    switch (_lives)
    //    {
    //        case 2:
    //            _rightEngine.SetActive(true);
    //            break;
    //        case 1:
    //            _leftEngine.SetActive(true);
    //            break;
    //        case 0:
    //            PlayerDied();
    //            break;
    //    }

    //}

    public void Damage()
    {
        if (_IsShieldsActive)
        {
            _shieldHealth -= 1;            

            if (_shieldHealth == 0)
            {
                //deactivate shields
                _IsShieldsActive = false;
                _shields.SetActive(false);

                _AudioSource.clip = _shieldsDownClip;
                _AudioSource.Play();
            }
            else
            {
                SetShieldImage();
            }

            return;
        }

        //somehow player lives was getting below 0
        if (_lives > 0)
        {
            _lives -= 1;
        }

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
        _playerExploding = true;
        _AudioSource.clip = _explosionClip;
        _AudioSource.Play();
        Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
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
            Vector3 laserStartingPosition = new Vector3(transform.position.x, transform.position.y + 1.05f, 0);
            Instantiate(_laserPrefab, laserStartingPosition, Quaternion.identity);

            _currentAmmo -= 1;
            UpdateAmmoUI();
        }

        //play the audio clip
        _AudioSource.clip = _laserShotAudioClip;
        _AudioSource.Play();
    }

   
    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");        
        
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(direction * (_speed * _thrusterMultiplier) * Time.deltaTime);
            ShowThrusterImage(true);
        }
        else
        {
            transform.Translate(direction * _speed * Time.deltaTime);

            //if thrusters <> active
            if (_speedBoostIsActive == false)
            {
                ShowThrusterImage(false);
            }
        }



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

    private void ShowThrusterImage(bool active)
    {
        _thrusters.SetActive(active);
    }

    IEnumerator PowerDownTripleShot()
    {
        yield return new WaitForSeconds(_tripleShotActiveLengthOfTime);
       
        _isTripleShotActive = false;                        

    }

    IEnumerator PowerDownSpeedBoost()
    {
        yield return new WaitForSeconds(_speedActiveLengthOfTime);        
        _speed /= _speedMultiplier;
        ShowThrusterImage(false);
        _speedBoostIsActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyLaser")
        {
            Damage();

            Destroy(other.gameObject);
        }
    }
}
