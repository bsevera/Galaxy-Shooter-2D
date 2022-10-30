using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{
    [SerializeField]
    private GameObject _explosionPrefab;

    [SerializeField]
    private PowerupRarity _rarity;

    public PowerupRarity Rarity
    {
        get { return _rarity; }
    }

    [SerializeField]
    private float _speed = 3.0f;

    private float _bottomOfScreen = -5.4f;
    private float _topOfScreen = 6.5f;
    private float _minX = -9.0f;
    private float _maxX = 9.0f;

    [SerializeField]
    private AudioClip _audioClip;

    //ID for powerups
    //0 = triple shot
    //1 = speed
    //2 = shields
    //3 = ammo
    //4 = extra life
    //5 = blossom laser
    //6 = lose all ammo

    [SerializeField]
    private int powerupID;

    [SerializeField]
    private float _collectionSpeed = 8.0f;
    private Player _player;

    // Start is called before the first frame update
    void Start()
    {
        GetPlayerReference();

        //position object at the top of the screen
        SetStartPosition();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y > _bottomOfScreen)
        {
            if (Input.GetKey(KeyCode.C))
            {
                MoveTowardPlayer();
            }

            //move the object            
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            //if object moves off of the bottom of screen, destroy it
            Destroy(this.gameObject);
        }
    }

    private void SetStartPosition()
    {
        float randomX = Random.Range(_minX, _maxX);

        transform.position = new Vector3(randomX, _topOfScreen, 0);
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

    private void MoveTowardPlayer()
    {
        if (_player != null)
        {
            float step = _collectionSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, step);
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.transform.GetComponent<Player>();

            //play sound effect
            AudioSource.PlayClipAtPoint(_audioClip, transform.position);
            
            if (player != null)
            {
                switch (powerupID)
                {
                    case 0:
                        player.TripleShotActive();
                        break;
                    case 1:
                        player.SpeedBoostActive();
                        break;
                    case 2:
                        player.ShieldsActive();
                        break;
                    case 3:
                        player.AmmoCollected();
                        break;
                    case 4:
                        player.ExtraLifeCollected();
                        break;
                    case 5:
                        player.BlossomLaserActive();
                        break;
                    case 6:
                        player.LoseAllAmmoCollected();
                        break;
                    default:
                        Debug.Log("Unknown powerupID value");
                        break;
                }

            }

            //destroy us
            Destroy(this.gameObject);

        }

        if (other.tag == "EnemyLaser")
        {
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

            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            Destroy(this.gameObject, 0.25f);
        }

    }
}
