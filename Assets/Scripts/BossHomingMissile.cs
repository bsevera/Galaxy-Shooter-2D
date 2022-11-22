using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHomingMissile : MonoBehaviour
{
    [SerializeField]
    private int _scoreValue = 20;

    [SerializeField]
    private BossHomingMissileLocation _hmLocation;

    [SerializeField]
    private AudioClip _thrusterClip;

    [SerializeField]
    private AudioClip _explosionClip;

    [SerializeField]
    private float _timeToLive = 20f;

    private float _speed = 4.0f;

    private GameObject _player;

    private Quaternion _targetPlayerRotation;

    private AudioSource _audioSource;
    private Animator _animator;

    private Boss _boss;
    

    // Start is called before the first frame update
    void Start()
    {
        GetAudioSourceReference();
        GetAnimatorReference();
        GetBossReference();

        SetThrusterAudio();

        _player = GameObject.FindGameObjectWithTag("Player");

        StartCoroutine(TimeToLive());

    }

    // Update is called once per frame
    void Update()
    {
        if (_boss == null)
        {
            Destroy(this.gameObject);
        }
        else
        {
            SetPlayerTarget();
        }
    }

    #region Startup References
    private void GetAudioSourceReference()
    {
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Boss Homing Missile :: Audio Source is Null");
        }
    }

    private void GetAnimatorReference()
    {
        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Boss Homing Missile :: Animator is Null");
        }
    }

    private void GetBossReference()
    {
        _boss = GameObject.FindGameObjectWithTag("Enemy").GetComponent<Boss>();
        if (_boss != null)
        {
            Debug.LogError("Boss Homing Missile :: Boss object is Null");
        }
    }
    #endregion

    private void SetThrusterAudio()
    {
        _audioSource.clip = _thrusterClip;
        _audioSource.Play();
    }

    private void SetPlayerTarget()
    {
        Vector3 missilePosition = transform.position;
        Vector3 playerPosition;

        playerPosition = _player.transform.position;
        _targetPlayerRotation = Quaternion.LookRotation(transform.forward, (playerPosition - missilePosition));

        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, playerPosition, step);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetPlayerRotation, _speed * Time.deltaTime);

    }

    public void SetLocation(BossHomingMissileLocation location)
    {
        _hmLocation = location;

        if (_hmLocation == BossHomingMissileLocation.Left)
        {
            SetLeftPosition();
        }
        else
        {
            SetRightPosition();
        }
    }

    private void SetLeftPosition()
    {
        transform.position = new Vector3(-1.8f, 2.8f, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 122));
    }

    private void SetRightPosition()
    {
        transform.position = new Vector3(2.0f, 2.8f, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -122));
    }

    private void PlayExplosionSoundClip()
    {
        _audioSource.clip = _explosionClip;
        _audioSource.Play();
    }

    IEnumerator TimeToLive()
    {
        yield return new WaitForSeconds(_timeToLive);
        Destroy(this.gameObject);
    }

    private void StopMovement()
    {
        _speed = 0;

        //destroy thruster image on missile
        Destroy(transform.GetChild(0).gameObject);
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser" || other.tag == "HomingMissile")
        {
            Laser laser = other.transform.GetComponent<Laser>();
            if (laser != null)
            {
                laser.StopLaserMovement();
            }

            StopMovement();

            _player.GetComponent<Player>().IncreaseScore(_scoreValue);

            //destroy laser
            Destroy(other.gameObject);

            //destroy animation
            _animator.SetTrigger("OnEnemyDeath");

            PlayExplosionSoundClip();

            Destroy(GetComponent<Collider2D>());

            Destroy(this.gameObject, 2.8f);            
        }
    }

}
