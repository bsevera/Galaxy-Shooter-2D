using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{    

    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private float _maxThrusterValue = 100f;
    [SerializeField]
    private float _currentThrusterValue;

    [SerializeField]
    private float _thrusterMultiplier = 1.5f;

    [SerializeField]
    private float _speedMultiplier = 2.0f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private GameObject _tripleShotPrefab;

    [SerializeField]
    private GameObject _homingMissilePrefab;

    [SerializeField]
    private bool _isTripleShotActive = false;

    [SerializeField]
    private bool _isBlossomLaserActive = false;

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
    private float _blossomLaserActiveLengthOfTime = 5.0f;

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

    [SerializeField]
    private AudioClip _loseAllAmmoClip;

    private AudioSource _AudioSource;

    private SpawnManager _spawnManager;
    //private int _wave = 0;

    [SerializeField]
    private int _score = 0;

    [SerializeField]
    private int _maxAmmo = 30;
    private int _currentAmmo;

    private int _homingMissileCount = 0;    

    private MainCamera _mainCamera;
    private UIManager _UIManager;    

    private bool _playerExploding = false;

    // Start is called before the first frame update
    void Start()
    {

        //take the current position and assign it a start position of 0, 0, 0 
        transform.position = new Vector3(0, 0, 0);

        GetSpawnManagerReference();
        GetMainCameraReference();
        GetUIManagerReference();
        GetAudioSourceReference();

        _currentAmmo = _maxAmmo;
        _currentThrusterValue = _maxThrusterValue;

        UpdateAmmoUI();
        UpdateHomingMissileUI();
        SetThrusterUIMaxValue();        

    }

    #region Startup References

    private void GetSpawnManagerReference()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();

        if (_spawnManager == null)
        {
            Debug.LogError("spawnManager is null!");
        }
    }

    private void GetMainCameraReference()
    {
        //Find the main camera object and cache it for later
        _mainCamera = GameObject.Find("Main Camera").GetComponent<MainCamera>();
        if (_mainCamera == null)
        {
            Debug.LogError("Camera is null");
        }
    }

    private void GetUIManagerReference()
    {
        //Find UIManager and cache it for later
        _UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_UIManager == null)
        {
            Debug.Log("UIManager is null");
        }
    }

    private void GetAudioSourceReference()
    {
        //get the audio component of the player and assign the audio clip for the fire laser sound
        _AudioSource = GetComponent<AudioSource>();
        if (_AudioSource == null)
        {
            Debug.LogError("Audio Source of the Player is Null");
        }
    }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (_playerExploding == false)
        {
            CalculateMovement();

            if ((_currentAmmo > 0 || _isTripleShotActive || _isBlossomLaserActive) && Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)            
            {
                FireLaser();
            }
            else if (_currentAmmo == 0 && Input.GetKeyDown(KeyCode.Space) && _isTripleShotActive == false && _isBlossomLaserActive == false && Time.time > _canFire)            
            {
                PlayNoAmmoClip();
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                StartCoroutine(IncreaseFuelRoutine());
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                if (_homingMissileCount == 0)
                {
                    PlayNoAmmoClip();
                }
                {
                    FireHomingMissile();
                }
            }
        }
    }

    #region Audio Methods
    private void PlayNoAmmoClip()
    {
        _AudioSource.clip = _noAmmoClip;
        _AudioSource.Play();
    }

    private void PlayLoseAllAmmoSoundEffect()
    {
        _AudioSource.clip = _loseAllAmmoClip;
        _AudioSource.Play();
    }

    private void PlayPowerUpSoundEffect()
    {
        _AudioSource.clip = _powerUpAudioClip;
        _AudioSource.Play();
    }

    #endregion

    #region Powerups Collected Methods
    public void LoseAllAmmoCollected()
    {
        PlayLoseAllAmmoSoundEffect();
        _currentAmmo = 0;
        UpdateAmmoUI();
    }

    public void HomingMissileCollected()
    {
        PlayPowerUpSoundEffect();
        _homingMissileCount += 3;
        UpdateHomingMissileUI();
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

    public void TripleShotActive()
    {
        PlayPowerUpSoundEffect();
        _isTripleShotActive = true;
        StartCoroutine(PowerDownTripleShot());
    }

    public void BlossomLaserActive()
    {
        PlayPowerUpSoundEffect();
        _isBlossomLaserActive = true;
        StartCoroutine(PowerDownBlossomLaser());
    }

    public void SpeedBoostActive()
    {
        PlayPowerUpSoundEffect();

        //don't activate speed boost if it is already active
        if (_speedBoostIsActive == false)
        {
            ShowThrusterImage(true);
            _speedBoostIsActive = true;
            _speed *= _speedMultiplier;
            StartCoroutine(PowerDownSpeedBoost());
        }
    }

    #endregion

    #region Shields Methods
    public void ShieldsActive()
    {
        PlayPowerUpSoundEffect();

        _shieldHealth = _shieldHealthMax;
        _IsShieldsActive = true;
        _shields.SetActive(true);

        //if (_IsShieldsActive)
        //{
        //    if (_shieldHealth < _shieldHealthMax)
        //    {
        //        _shieldHealth += 1;
        //    }
        //}
        //else
        //{
        //    _shieldHealth = _shieldHealthMax;
        //    _IsShieldsActive = true;
        //    _shields.SetActive(true);
        //}

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

    #endregion

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

    public void KilledAnEnemy()
    {
        _spawnManager.OnEnemyKilled();
    }

    #region UI Update Methods

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

    private void UpdateHomingMissileUI()
    {
        if (_UIManager != null)
        {
            _UIManager.UpdateHomingMissileCount(_homingMissileCount);
        }
    }

    private void SetThrusterUIMaxValue()
    {
        _UIManager.SetThrusterGaugeMax(_maxThrusterValue);
    }

    private void UpdateThrusterUI()
    {
        _UIManager.UpdateThrusterGauge(_currentThrusterValue);
    }
    #endregion

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

        //reduce lives by one if > 0
        if (_lives > 0)
        {
            _lives -= 1;
        }

        //update lives count in UI
        _UIManager.UpdateLives(_lives);

        switch (_lives)
        {
            case 2:
                _AudioSource.clip = _explosionClip;
                _AudioSource.Play();

                _rightEngine.SetActive(true);
                break;
            case 1:
                _AudioSource.clip = _explosionClip;
                _AudioSource.Play();

                _leftEngine.SetActive(true);
                break;
            case 0:
                PlayerDied();
                break;
        }

        //shake camera after being hit
        _mainCamera.ShakeCamera();

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
        if (_AudioSource.enabled == true)
        {
            _AudioSource.clip = _explosionClip;
            _AudioSource.Play();
        }
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
        else if (_isBlossomLaserActive) 
        {
            FireBlossomLaser();
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

    private void FireHomingMissile()
    {

        Instantiate(_homingMissilePrefab, transform.position, Quaternion.identity);
        _homingMissileCount -= 1;
        UpdateHomingMissileUI();
    }

    private void FireBlossomLaser()
    {
        //five lasers shoot in different directions
        
        //left laser
        GameObject leftLaser = Instantiate(_laserPrefab);
        leftLaser.GetComponent<Laser>().SetDirection(LaserDirection.Left);
        Vector3 leftLaserStartingPosition = new Vector3(transform.position.x, transform.position.y + 1.05f, 0);
        leftLaser.transform.position = leftLaserStartingPosition;
        leftLaser.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));

        //left upper laser
        GameObject upperLeftLaser = Instantiate(_laserPrefab);
        upperLeftLaser.GetComponent<Laser>().SetDirection(LaserDirection.UpperLeft);
        Vector3 upperLeftLaserStartingPosition = new Vector3(transform.position.x, transform.position.y + 1.05f, 0);
        upperLeftLaser.transform.position = upperLeftLaserStartingPosition;
        upperLeftLaser.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));

        //center laser
        GameObject centerLaser = Instantiate(_laserPrefab);
        centerLaser.GetComponent<Laser>().SetDirection(LaserDirection.Up);
        Vector3 centerLaserStartingPosition = new Vector3(transform.position.x, transform.position.y + 1.05f, 0);
        centerLaser.transform.position = centerLaserStartingPosition;

        //right upper laser
        GameObject upperRightLaser = Instantiate(_laserPrefab);
        upperRightLaser.GetComponent<Laser>().SetDirection(LaserDirection.UpperRight);
        Vector3 upperRightLaserStartingPosition = new Vector3(transform.position.x, transform.position.y + 1.05f, 0);
        upperRightLaser.transform.position = upperLeftLaserStartingPosition;
        upperRightLaser.transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));


        //right laser
        GameObject rightLaser = Instantiate(_laserPrefab);
        rightLaser.GetComponent<Laser>().SetDirection(LaserDirection.Right);
        Vector3 rightLaserStartingPosition = new Vector3(transform.position.x, transform.position.y + 1.05f, 0);
        rightLaser.transform.position = rightLaserStartingPosition;
        rightLaser.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 90));

        _AudioSource.clip = _laserShotAudioClip;
        _AudioSource.Play();
    }


    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        if (Input.GetKey(KeyCode.LeftShift) && ThrusterFuelEmpty() == false)
        {
            transform.Translate(direction * (_speed * _thrusterMultiplier) * Time.deltaTime);
            ShowThrusterImage(true);

            //only decrease thruster gauge if speed boost is not active
            if (_speedBoostIsActive == false)
            {
                DecreaseThrusterFuel();                
            }
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

        //constrain the object to not move past 9 and -9 on the horizontal
        //constrain the object to not move past 0 and -3.5 on the vertical
        if (transform.position.x >= 9)
        {
            transform.position = new Vector3(9, transform.position.y, 0);
        }
        else if (transform.position.x <= -9)
        {
            transform.position = new Vector3(-9, transform.position.y, 0);
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

    #region Thruster Methods
    private void ShowThrusterImage(bool active)
    {
        _thrusters.SetActive(active);
    }

    private void IncreaseThrusterFuel()
    {
        _currentThrusterValue += 0.5f;
        _UIManager.UpdateThrusterGauge(_currentThrusterValue);
    }

    private void DecreaseThrusterFuel()
    {
        _currentThrusterValue -= 0.2f;
        _UIManager.UpdateThrusterGauge(_currentThrusterValue);
    }

    private bool ThrusterFuelFull()
    {
        if (_currentThrusterValue == _maxThrusterValue)
        {
            return true;  
        }
        else
        {
            return false;
        }
    }

    private bool ThrusterFuelEmpty()
    {
        if (_currentThrusterValue == 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion

    #region Coroutines

    IEnumerator PowerDownTripleShot()
    {
        yield return new WaitForSeconds(_tripleShotActiveLengthOfTime);
       
        _isTripleShotActive = false;                        

    }

    IEnumerator PowerDownBlossomLaser()
    {
        yield return new WaitForSeconds(_blossomLaserActiveLengthOfTime);
        _isBlossomLaserActive = false;
    }

    IEnumerator PowerDownSpeedBoost()
    {
        yield return new WaitForSeconds(_speedActiveLengthOfTime);        
        _speed /= _speedMultiplier;
        ShowThrusterImage(false);
        _speedBoostIsActive = false;
    }

    IEnumerator IncreaseFuelRoutine()
    {
        while (ThrusterFuelFull() == false)
        {
            IncreaseThrusterFuel();
            yield return new WaitForSeconds(.2f);
        }
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "EnemyLaser")
        {
            Damage();

            Destroy(other.gameObject);
        }
    }
}
