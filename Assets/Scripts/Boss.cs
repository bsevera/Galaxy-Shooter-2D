using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    [SerializeField]
    private GameObject _bossLaserPrefab;

    [SerializeField]
    private GameObject _bossHomingMissilePrefab;

    [SerializeField]
    private float _yStartPosition = 10f;

    [SerializeField]
    private float _yStopPosition = 6f;

    [SerializeField]
    private float _speed = 3f;

    [SerializeField]
    private int _maxBossHealth = 60;
    private int _currentBossHealth;

    [SerializeField]
    private AudioClip _explosionClip;

    [SerializeField]
    private int _hitScoreValue;

    private UIManager _UIManager;
    private AudioSource _audioSource;

    private bool _healthGaugeVisible = false;

    private Player _player;
    private Animator _animator;

    [SerializeField]
    private float _homingMissileFireRate = 2.5f;
    private float _canFireHomingMissile = 0.0f;
    //private BossHomingMissileLocation _lastHMFired = BossHomingMissileLocation.Right;
    
    private bool _canFireLongLaser = true;
    private bool _hasStoppedMoving = false;
    private BossLaserLocation _lastLaserFired = BossLaserLocation.Left;
    
    // Start is called before the first frame update
    void Start()
    {
        GetUIManagerReference();
        GetAudioSourceReference();
        GetAnimatorReference();
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

    private void GetAnimatorReference()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Boss :: Animator is Null");
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

            //destroy animation
            _animator.SetTrigger("OnEnemyDeath");

            PlayExplosionSoundClip();

            Destroy(GetComponent<Collider2D>());

            Destroy(this.gameObject, 2.8f);
        }
    }

    public void LaserPoweredDown()
    {
        _canFireLongLaser = true;
    }
}
