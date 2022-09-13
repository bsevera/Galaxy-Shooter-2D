using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    [SerializeField]
    private GameObject _enemyLaserPrefab;

    private float _bottomOfScreen = -5.4f;
    private float _topOfScreen = 6.5f;
    private float _minX = -9.0f;
    private float _maxX = 9.0f;

    private Player _player;
    private Animator _animator;
    private bool _enemyIsDestroyed = false;
    private float _fireRate = -1;
    private float _canFire;

    private AudioSource _audioSource;

    // Start is called before the first frame update
    void Start()
    {
        //get a reference to the player object
        _player = GameObject.Find("Player").GetComponent<Player>();
        if (_player == null)
        {
            Debug.LogError("Player object is null");
        }

        _animator = GetComponent<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator component of enemy object is null");
        }

        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Enemy::Audio Source is null");
        }

        //position object at the top of the screen
        SetStartPosition();

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();


        if ((Time.time > _canFire) && _enemyIsDestroyed == false)
        {
            _fireRate = Random.Range(2f, 4f);
            _canFire = Time.time + _fireRate;
            FireLaser();            
        }
    }

    private void CalculateMovement()
    {
        if (transform.position.y > _bottomOfScreen)
        {
            //move down 4 meters per second
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            //if object moves off of the bottom of screen, respawn it at the top with a new random x position
            SetStartPosition();
        }

    }

    private void SetStartPosition()
    {
        float randomX = Random.Range(_minX, _maxX);

        transform.position = new Vector3(randomX, _topOfScreen, 0);
    }

    private void FireLaser()
    {
        Vector3 laserStartingPosition = new Vector3(transform.position.x, transform.position.y - 1.05f, 0);
        GameObject enemyLaser = Instantiate(_enemyLaserPrefab, laserStartingPosition, Quaternion.identity);
        Laser[] lasers = enemyLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].AssignEnemyLaser();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            //get player object
            Player player = other.transform.GetComponent<Player>();

            //damage the player
            if (player != null)
            {
                player.Damage();
            }

            //set speed to 0 before starting the animation
            _speed = 0f;

            //destroy animation
            _animator.SetTrigger("OnEnemyDeath");

            //destroy us
            DestroyUs();            
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

            //add 10 to score
            if (_player != null)
            {
                _player.IncreaseScore(10);
            }

            _enemyIsDestroyed = true;

            //set speed to 0 before starting the animation
            _speed = 0f;

            //destroy animation
            _animator.SetTrigger("OnEnemyDeath");

            //destroy us
            DestroyUs();            
        }
    }

    private void DestroyUs()
    {

        _audioSource.Play();

        //fix to not allow destroyed enemy to be hit again        
        Destroy(GetComponent<Collider2D>());

        Destroy(this.gameObject, 2.8f);
        
    }

}
