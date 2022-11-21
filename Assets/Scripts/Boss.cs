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
    private int _maxBossHealth = 30;
    private int _currentBossHealth;

    private UIManager _UIManager;
    private AudioSource _audioSource;

    private bool _healthGaugeVisible = false;

    [SerializeField]
    private float _laserFireRate = 20f;

    [SerializeField]
    private float _homingMissileFireRate = 15f;

    //private float _canFireLaser;
    private bool _canFireLaser = true;
    private bool _hasStoppedMoving = false;
    private BossLaserLocation _lastLaserFired = BossLaserLocation.Left;
    
    // Start is called before the first frame update
    void Start()
    {
        GetUIManagerReference();
        GetAudioSourceReference();

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
            WeaponsController();
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
        if (_canFireLaser)
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

            _canFireLaser = false;
        }
    }

    //private void WeaponsController()
    //{
    //    Debug.Log("WeaponsController :: Time.time = " + Time.time);
    //    Debug.Log("WeaponsController :: CanFirelaser = " + _canFireLaser.ToString());

    //    if (Time.time > _canFireLaser)
    //    {            
    //        _canFireLaser = Time.time + _laserFireRate;

    //        if (_lastLaserFired == BossLaserLocation.Right)
    //        {
    //            FireLeftLaser();
    //            _lastLaserFired = BossLaserLocation.Left;
    //        }
    //        else
    //        {
    //            FireRightLaser();
    //            _lastLaserFired = BossLaserLocation.Right;
    //        }
    //    }

    //}

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

    }

    private void FireRightHomingMissile()
    {

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

            ApplyDamage();

        }
        else if (other.tag == "Homing Missile")
        {
            HomingMissile homingMissile = other.transform.GetComponent<HomingMissile>();
            if (homingMissile != null)
            {
                homingMissile.StopMovement();
            }
            
            //destroy homing missile
            Destroy(other.gameObject);

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

    private void ApplyDamage()
    {
        _currentBossHealth -= 1;
        _UIManager.UpdateBossHealth(_currentBossHealth);

        if (_currentBossHealth == 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void LaserPoweredDown()
    {
        _canFireLaser = true;
    }
}
