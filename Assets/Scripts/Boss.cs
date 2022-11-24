using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [Header("Weapons Prefabs")]
    [SerializeField]
    private GameObject _bossLaserPrefab;

    [SerializeField]
    private GameObject _bossHomingMissilePrefab;


    [Header("Weapons - Additional Settings")]
    [SerializeField]
    private float _homingMissileFireRate = 2.5f;


    [Header("Movement")]
    [SerializeField]
    private float _yStartPosition = 10f;

    [SerializeField]
    private float _yStopPosition = 6f;

    [SerializeField]
    private float _speed = 3f;


    [Header("Health")]
    [SerializeField]
    private int _maxBossHealth = 60;
    private int _currentBossHealth;


    [Header("Destruction of Object")]
    [SerializeField]
    private AudioClip _explosionClip;

    [SerializeField]
    private GameObject _explosionPrefab;

    [Header("Scoring")]
    [SerializeField]
    private int _hitScoreValue;


    private UIManager _UIManager;
    private AudioSource _audioSource;

    private bool _healthGaugeVisible = false;

    private Player _player;

    private float _canFireHomingMissile = 0.0f;
    
    private bool _canFireLongLaser = true;
    private bool _hasStoppedMoving = false;
    private BossLaserLocation _lastLaserFired = BossLaserLocation.Left;
    
    // Start is called before the first frame update
    void Start()
    {
        GetUIManagerReference();
        GetAudioSourceReference();        
        GetPlayerReference();

        SetStartPosition();
    }

    private void SetHealthandHealthGauge()
    {
        _currentBossHealth = _maxBossHealth;
        SetHealthGauge();
        _healthGaugeVisible = true;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        if (_hasStoppedMoving)
        {
            if (_player != null)
            {
                WeaponsController();
            }
        }
    }

    #region Startup References

    private void GetUIManagerReference()
    {
        //Find UIManager and cache it for later
        _UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();
        if (_UIManager == null)
        {
            Debug.Log("Boss :: UIManager is null");
        }
    }

    private void GetAudioSourceReference()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Boss :: Audio Source is Null");
        }
    }

    private void GetPlayerReference()
    {
        //get a reference to the player object
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Boss :: Player object is null");
        }
    }

    #endregion

    #region Movement Methods
    private void SetStartPosition()
    {
        //start the position of the boss off screen
        transform.position = new Vector3(0, _yStartPosition, 0);
    }

    private void CalculateMovement()
    {
        if (transform.position.y > _yStopPosition)
        {
            //move down
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            if (_healthGaugeVisible == false)
            {
                SetHealthandHealthGauge();
            }

            StopAudioSourceLoop();
            _hasStoppedMoving = true;
        }

    }
    #endregion

    #region Fire Methods

    private void WeaponsController()
    {
        //lasers
        if (_canFireLongLaser)
        {
            if (_lastLaserFired == BossLaserLocation.Right)
            {
                FireLeftLaser();
                _lastLaserFired = BossLaserLocation.Left;
            }
            else
            {
                FireRightLaser();
                _lastLaserFired = BossLaserLocation.Right;
            }

            _canFireLongLaser = false;
        }

        //homing missile
        if (Time.time > _canFireHomingMissile)
        {
            _homingMissileFireRate = 5f;
            _canFireHomingMissile = Time.time + _homingMissileFireRate;

            if (Random.Range(0, 2) == 0)
            {
                FireLeftHomingMissile();
            }
            else
            {
                FireRightHomingMissile();
            }
        }
    }

    private void FireLeftLaser()
    {
        GameObject laser = Instantiate(_bossLaserPrefab);
        laser.GetComponent<BossLaser>().SetLocation(BossLaserLocation.Left);
    }

    private void FireRightLaser()
    {
        GameObject laser = Instantiate(_bossLaserPrefab);
        laser.GetComponent<BossLaser>().SetLocation(BossLaserLocation.Right);
    }

    private void FireLeftHomingMissile()
    {
        GameObject homingMissile = Instantiate(_bossHomingMissilePrefab);
        homingMissile.GetComponent<BossHomingMissile>().SetLocation(BossHomingMissileLocation.Left);
    }

    private void FireRightHomingMissile()
    {
        GameObject homingMissile = Instantiate(_bossHomingMissilePrefab);
        homingMissile.GetComponent<BossHomingMissile>().SetLocation(BossHomingMissileLocation.Right);
    }

    #endregion

    private void StopAudioSourceLoop()
    {
        _audioSource.loop = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
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

            _player.IncreaseScore(_hitScoreValue);

            ApplyDamage();

        }
        else if (other.tag == "HomingMissile")
        {
            HomingMissile homingMissile = other.transform.GetComponent<HomingMissile>();
            if (homingMissile != null)
            {
                homingMissile.StopMovement();
            }
            
            //destroy homing missile
            Destroy(other.gameObject);

            _player.IncreaseScore(_hitScoreValue);

            ApplyDamage();
        }
    }

    private void SetHealthGauge()
    {
        if (_UIManager != null)
        {
            _UIManager.SetBossHealthMax(_maxBossHealth);
        }
    }

    private void PlayExplosionSoundClip()
    {
        _audioSource.clip = _explosionClip;
        _audioSource.Play();
    }

    private void ApplyDamage()
    {
        _currentBossHealth -= 1;
        _UIManager.UpdateBossHealth(_currentBossHealth);

        if (_currentBossHealth == 0)
        {

            Vector3 explodeFrontCenterPos = new Vector3(transform.position.x, transform.position.y + 2, transform.position.z);
            Vector3 explodeRearCenterPos = new Vector3(transform.position.x, transform.position.y - 2, transform.position.z);
            Vector3 explodeLeftPos = new Vector3(transform.position.x - 3, transform.position.y - 2, transform.position.z);
            Vector3 explodeRightPos = new Vector3(transform.position.x + 3, transform.position.y - 3, transform.position.z);

            Instantiate(_explosionPrefab, explodeFrontCenterPos, Quaternion.identity);
            Instantiate(_explosionPrefab, explodeLeftPos, Quaternion.identity);
            Instantiate(_explosionPrefab, explodeRightPos, Quaternion.identity);
            Instantiate(_explosionPrefab, explodeRearCenterPos, Quaternion.identity);

            PlayExplosionSoundClip();

            Destroy(GetComponent<Collider2D>());

            Destroy(this.gameObject, 0.25f);
        }
    }

    public void LaserPoweredDown()
    {
        _canFireLongLaser = true;
    }

}
