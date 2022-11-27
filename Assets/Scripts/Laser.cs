using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    [SerializeField]
    private LaserDirection _laserDirection;

    // Update is called once per frame
    void Update()
    {
        MoveLaser();
    }

    private void MoveLaser()
    {
        switch (_laserDirection)
        {
            case LaserDirection.Left:
                MoveLeft();
                break;
            case LaserDirection.UpperLeft:
                MoveUpAndLeft();
                break;
            case LaserDirection.Up:
                MoveUp();
                break;
            case LaserDirection.UpperRight:
                MoveUpAndRight();
                break;
            case LaserDirection.Right:
                MoveRight();
                break;
            case LaserDirection.Down:
                MoveDown();
                break;
            case LaserDirection.DownLeft:
                MoveDownLeft();
                break;
            case LaserDirection.DownRight:
                MoveDownRight();
                break;
        }
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //if position on the Y is less than -4, destroy the object
        if (transform.position.y < -8)
        {
            DestroyMe();
        }
    }

    private void MoveDownLeft()
    {        
        Vector3 newPosition = VectorFromAngle(180);
        transform.Translate(newPosition * _speed * Time.deltaTime, Space.World);

        if (transform.position.x < -9 || transform.position.y < -8)
        {
            DestroyMe();
        }
    }

    private void MoveDownRight()
    {
        Vector3 newPosition = VectorFromAngle(-45);
        transform.Translate(newPosition * _speed * Time.deltaTime, Space.World);

        if (transform.position.x > 9 || transform.position.y < -8) 
        {
            DestroyMe();
        }
    }

    private void MoveLeft()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime, Space.World);
        if (transform.position.x < -9)
        {
            DestroyMe();
        }
    }

    private void MoveUpAndLeft()
    {
        Vector3 newPosition = VectorFromAngle(90);
        transform.Translate(newPosition * _speed * Time.deltaTime, Space.World);

        if (transform.position.x < -9 || transform.position.y > 9)
        {
            DestroyMe();
        }
    }

    private void MoveUp()
    {
        //move laser up
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        //if position on the Y is greater than 8, destroy the object
        if (transform.position.y > 8f)
        {
            DestroyMe();
        }
    }

    private void MoveUpAndRight()
    {
        Vector3 newPosition = VectorFromAngle(45);
        transform.Translate(newPosition * _speed * Time.deltaTime, Space.World);

        if (transform.position.x > 9 || transform.position.y > 9)
        {
            DestroyMe();
        }
    }

    private void MoveRight()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime, Space.World);
        if (transform.position.x > 9)
        {
            DestroyMe();
        }
    }

    private void DestroyMe()
    {
        if (this.transform.parent != null && this.transform.parent.childCount == 1)
        {
            Destroy(this.transform.parent.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    Vector3 VectorFromAngle(float theta)
    {
        return new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);
    }

    public void SetDirection(LaserDirection direction)
    {
        _laserDirection = direction;
    }

    public void StopLaserMovement()
    {
        _speed = 0;
    }
}
