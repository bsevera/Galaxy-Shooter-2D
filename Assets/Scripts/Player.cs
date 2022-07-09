using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _speed = 5f;

    [SerializeField]
    private GameObject _laserPrefab;

    [SerializeField]
    private float _fireRate = 0.15f;

    //cool down system for limiting how quickly the player can fire
    private float _canFire = -1f;

    // Start is called before the first frame update
    void Start()
    {
        //take the current position and assign it a start position of 0, 0, 0 
        transform.position = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    //private void Update()
    //{
    //    float horizontalInput = Input.GetAxis("Horizontal");
    //    float verticalInput = Input.GetAxis("Vertical");

    //    Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
    //    transform.Translate(direction * _speed * Time.deltaTime);

    //}

    void Update()
    {

        SetSpeed();
        CalculateMovement();

        if (Input.GetKeyDown(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

    }

    void FireLaser()
    {
        _canFire = Time.time + _fireRate;

        //laser needs to be spawned .8f above the player object
        Vector3 laserStartingPosition = new Vector3(transform.position.x, transform.position.y + .8f, 0);
        Instantiate(_laserPrefab, laserStartingPosition, Quaternion.identity);

    }

    void SetSpeed()
    {
        if (Input.GetKeyDown(KeyCode.Equals)) //plus key is located on the equals key
        {
            if (_speed < 11.0f)
            {
                _speed += 1.0f;
                Debug.Log("Speed = " + _speed);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Minus))
        {
            if (_speed > 0.0f)
            {
                _speed -= 1.0f;
                Debug.Log("Speed = " + _speed);
            }
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

        //constrain the object to not move past 9 and -9 on the horizontal
        //constrain the object to not move past 0 and -3 on the vertical
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
        else if (transform.position.y <= -3)
        {
            transform.position = new Vector3(transform.position.x, -3, 0);
        }


        //bonus - wrap the object so if you go beyond the bounds on the right, it appears on the left and vice versa
        //bonus - wrap the object so if you go beyond the vertical bounds on the top, it appears on the bottom and vice versa
        //if (transform.position.x >= 9)
        //{
        //    transform.position = new Vector3(-9, transform.position.y, 0);
        //}
        //else if (transform.position.x <= -9)
        //{
        //    transform.position = new Vector3(9, transform.position.y, 0);
        //}

        //if (transform.position.y > 0)
        //{
        //    transform.position = new Vector3(transform.position.x, -3, 0);
        //}
        //else if (transform.position.y <= -3)
        //{
        //    transform.position = new Vector3(transform.position.x, 0, 0);
        //}

    }
}
