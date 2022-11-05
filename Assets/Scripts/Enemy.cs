using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    [SerializeField]
    private float _dodgeSpeed = 5.0f;

    [SerializeField]
    private float _rammingSpeed = 6.0f;

    [SerializeField]
    private GameObject _enemyLaserPrefab;

    [SerializeField]
    private GameObject _enemyRearLaserPrefab;

    private float _bottomOfScreen = -5.4f;
    private float _topOfScreen = 6.5f;
    private float _minX = -9.0f;
    private float _maxX = 9.0f;


    private SpawnManager _spawnManager;
    private Player _player;
    private Animator _animator;
    private bool _enemyIsDestroyed = false;
    private float _fireRate = -1;
    private float _canFire;

    private float _rearFireRate = -1;
    private float _canFireBehind;

    private AudioSource _audioSource;

    [SerializeField]
    private EnemyMovementPattern _MovementPattern;
    private float _spawnTime;
    private float _frequency;
    private float _phase;
    private float _distanceY;

    [SerializeField]
    private bool _IsShieldsActive = false;
    [SerializeField]
    private GameObject _shields;

    [SerializeField]
    private EnemyType _enemyType;

    [SerializeField]
    private GameObject _detectorPrefab;

    [SerializeField]
    private DetectionType _detectionType;
    private GameObject _detector;

    private void Awake()
    {
        //randomly assign the movement pattern
        int count = Enum.GetValues(typeof(EnemyMovementPattern)).Length;
        int movementIndex = UnityEngine.Random.Range(0, count);
        _MovementPattern = (EnemyMovementPattern)Enum.GetValues(typeof(EnemyMovementPattern)).GetValue(movementIndex);

    }

    // Start is called before the first frame update
    void Start()
    {
        GetPlayerReference();

        GetSpawnManagerReference();

        GetAnimatorReferences();

        GetAudioSourceReference();

        if (_MovementPattern == EnemyMovementPattern.ZigZagDown)
        {
            _spawnTime = Time.time;
            _frequency = (float)(Math.PI * UnityEngine.Random.Range(0.16f, 0.64f));
            _phase = UnityEngine.Random.Range(0f, 2f);
        }

        //if the enemy ship has shields assigned to it, randomly assign whether the shield will be turned on
        if (HasShields())
        {
            if (UnityEngine.Random.Range(0, 2) == 1)
            {
                EnableShields();
            }
        }

        GetDetectorReference();

        //position object at the top of the screen
        SetStartPosition();

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        //if detection type = powerup or powerup and laser
        if (_detectionType == DetectionType.Powerup || _detectionType == DetectionType.PowerupAndLaser)
        {
            if (_detector != null)
            {
                if (_detector.GetComponent<Detection>().PowerupDetected)
                {
                    FireLaser();
                }
            }
        }

        if (_detectionType == DetectionType.PlayerBehind)
        {
            if (_detector != null)
            {
                if (_detector.GetComponent<Detection>().PlayerDetected)
                {
                    if ((Time.time > _canFireBehind) && _enemyIsDestroyed == false)
                    {
                        _rearFireRate = UnityEngine.Random.Range(2f, 4f);
                        _canFireBehind = Time.time + _rearFireRate;
                        FireRearLaser();
                    }
                }
            }
        }

        if ((Time.time > _canFire) && _enemyIsDestroyed == false)
        {
            _fireRate = UnityEngine.Random.Range(2f, 4f);
            _canFire = Time.time + _fireRate;
            FireLaser();            
        }
    }

    #region Get Startup References

    private void GetDetectorReference()
    {
        if (_detectionType != DetectionType.None)
        {
            if (_detectorPrefab != null)
            {
                _detector = Instantiate(_detectorPrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.LogError("Enemy :: Detection Type is not None :: Detector is null");
            }
        }

    }

    private void GetAudioSourceReference()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Enemy::Audio Source is null");
        }
    }

    private void GetSpawnManagerReference()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent<SpawnManager>();
        if (_spawnManager == null)
        {
            Debug.LogError("Enemy :: Spawn Manager is null");
        }
    }

    private void GetPlayerReference()
    {
        //get a reference to the player object
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player object is null");
        }
    }

    private void GetAnimatorReferences()
    {
        if (HasShields())
        {
            _animator = GetComponent<Animator>();
        }
        else
        {
            if (this.gameObject.transform.childCount == 1)
            {
                //if game object has a child object holding the sprite, get the animator on that child object
                _animator = transform.GetChild(0).GetComponent<Animator>();
            }
            else
            {
                _animator = GetComponent<Animator>();
            }
        }

        if (_animator == null)
        {
            Debug.LogError("Animator component of enemy object is null");
        }

    }
    #endregion

    #region Shields Methods
    private void EnableShields()
    {
        _IsShieldsActive = true;
        _shields.SetActive(true);
    }

    private void DisableShields()
    {
        _IsShieldsActive = false;
        _shields.SetActive(false);
    }

    private bool HasShields()
    {
        if (_shields != null)
        { 
            return true; 
        }
        else 
        {
            return false; 
        }
    }
    #endregion
    
    private void CalculateMovement()
    {
        if (_detectionType == DetectionType.PlayerBehind)
        {
            if (_detector != null)
            {
                _detector.transform.position = transform.position;
            }
        }

        if (_detectionType == DetectionType.Laser || _detectionType == DetectionType.PowerupAndLaser)
        {            
            if (_detector != null)
            {
                //move detector object with the enemy
                _detector.transform.position = transform.position;
                if (_detector.GetComponent<Detection>().LaserDetected)
                {                    
                    AvoidLaser();
                }
            }
        }

        if (_enemyType == EnemyType.Aggressor)
        {
            MoveAggressively();
        }

        if (_MovementPattern == EnemyMovementPattern.Down)
        {
            MoveDown();
        }
        else
        {
            MoveZigZagDown();
        }
    }

    private void AvoidLaser()
    {
        //randomly decide the direction to move
        int direction = UnityEngine.Random.Range(0, 2);

        if (direction == 0)
        {
            //move left
            transform.Translate(Vector3.left * _dodgeSpeed * Time.deltaTime);
        }
        else
        {
            //move right
            transform.Translate(Vector3.right * _dodgeSpeed * Time.deltaTime);
        }
    }

    private void MoveAggressively()
    {
        if (_detector != null)
        {
            //set position of detector object to be that of the enemy
            _detector.transform.position = transform.position;

            //get the player detector script from the detector object
            Detection playerDector = _detector.GetComponent<Detection>();

            if (playerDector != null)
            {
                if (playerDector.PlayerDetected)
                {
                    if (_player != null && _player.transform.position.y < transform.position.y)
                    {
                        //calculate the direction and angle to the player object
                        //Vector3 playerDirection = _player.transform.position - transform.position;
                        //float angle = Mathf.Atan2(playerDirection.y, playerDirection.x) * Mathf.Rad2Deg + 90f;

                        //rotate the enemy toward the player

                        float step = _rammingSpeed * Time.deltaTime;
                        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, step);
                    }
                }
            }
        }
    }

    private void MoveDown()
    {
        if (transform.position.y > _bottomOfScreen)
        {
            //move down
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            //if object moves off of the bottom of screen, respawn it at the top with a new random x position
            SetStartPosition();
        }
    }

    private void MoveZigZagDown()
    {
        if (transform.position.y > _bottomOfScreen)
        {
            _distanceY = _speed * Mathf.Sin(_frequency * Time.time - _spawnTime + _phase) * Time.deltaTime;
            transform.Translate(Vector3.right * _distanceY);
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            SetStartPosition();
        }
    }

    private void SetStartPosition()
    {
        float randomX = UnityEngine.Random.Range(_minX, _maxX);

        transform.position = new Vector3(randomX, _topOfScreen, 0);
    }

    private void FireLaser()
    {
        Vector3 laserStartingPosition = new Vector3(transform.position.x, transform.position.y - 1.05f, 0);
        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, laserStartingPosition, Quaternion.identity);
    }

    private void FireRearLaser()
    {
        Vector3 laserStartingPosition = new Vector3(transform.position.x, transform.position.y + 1.3f, 0);
        GameObject enemyLaser = Instantiate(_enemyRearLaserPrefab, laserStartingPosition, Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Player")
        {
            Debug.Log("Enemy :: OnTriggerEnter2D :: Player");

            //get player object
            Player player = other.transform.GetComponent<Player>();

            //damage the player
            if (player != null)
            {
                player.Damage();

                if (HasShields() && _IsShieldsActive)
                {
                    DisableShields();
                }
                else
                {
                    _spawnManager.OnEnemyKilled();

                    //set speed to 0 before starting the animation
                    _speed = 0f;

                    //destroy animation
                    _animator.SetTrigger("OnEnemyDeath");

                    //destroy us
                    DestroyUs();
                }
            }
        }

        if (other.tag == "Laser")
        {
            //stop laser from moving past the enemy when it collides
            Laser laser = other.transform.GetComponent<Laser>();
            if (laser != null)
            {
                laser.StopLaserMovement();
            }

            //destroy laser
            if (other.transform.parent != null)
            {
                //triple shot - destroy parent
                Destroy(other.transform.parent.gameObject);
            }
            else
            {
                //standard laser                
                Destroy(other.gameObject);
            }

            //check to see if shield is active
            if (HasShields() && _IsShieldsActive)
            {
                DisableShields();
            }
            else
            {
                //add 10 to score
                if (_player != null)
                {
                    _player.IncreaseScore(10);
                }
                _spawnManager.OnEnemyKilled();

                _enemyIsDestroyed = true;

                //set speed to 0 before starting the animation
                _speed = 0f;

                //destroy animation
                _animator.SetTrigger("OnEnemyDeath");

                //destroy us
                DestroyUs();
            }
        }
    }

    private void DestroyUs()
    {
        _audioSource.Play();

        if (_detectionType != DetectionType.None)
        //if (_enemyType == EnemyType.Aggressor)
        {
            if (_detector != null)
            {
                Destroy(_detector);
            }
        }

        //fix to not allow destroyed enemy to be hit again        
        Destroy(GetComponent<Collider2D>());

        Destroy(this.gameObject, 2.8f);        
    }

}
