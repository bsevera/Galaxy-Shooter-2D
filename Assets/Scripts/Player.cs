using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    //public or private variable
    //if private only this script knows that it exists
    //four common data types in c#: int, float, bool, string
    //every variable has a name
    //option value assigned

    [SerializeField]

    private float _speed = 5f;

    // Start is called before the first frame update
    void Start()
    {
        //take the current position and assign it a start position of 0, 0, 0 
        transform.position = new Vector3(0, 0, 0);

    }


    // Update is called once per frame
    private void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);
        transform.Translate(direction * _speed * Time.deltaTime);

    }

    //void Update()
    //{

    //    SetSpeed();
    //    CalculateMovement();

    //}

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
