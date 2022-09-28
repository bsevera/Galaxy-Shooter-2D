using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;

    [SerializeField]
    private LaserDirection _laserDirection;

    private bool _isEnemyLaser = false;


    // Update is called once per frame
    void Update()
    {

        if (_isEnemyLaser)
        {
            MoveDown();
        }
        else
        {
            MoveLaser();            
        }

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
        }
    }

    private void MoveDown()
    {
        transform.Translate(Vector3.down * _speed * Time.deltaTime);

        //if position on the Y is less than -4, destroy the object
        if (transform.position.y < -4)
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }

    }

    private void MoveLeft()
    {
        transform.Translate(Vector3.left * _speed * Time.deltaTime, Space.World);
        if (transform.position.x < -9)
        {
            Destroy(this.gameObject);
        }
    }

    private void MoveUpAndLeft()
    {
        Vector3 newPosition = VectorFromAngle(90);
        transform.Translate(newPosition * _speed * Time.deltaTime, Space.World);

        if (transform.position.x < -9 || transform.position.y > 9)
        {
            Destroy(this.gameObject);
        }
    }

    private void MoveUp()
    {
        //move laser up
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        //if position on the Y is greater than 8, destroy the object
        if (transform.position.y > 8f)
        {
            if (this.transform.parent != null)
            {
                Destroy(this.transform.parent.gameObject);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void MoveUpAndRight()
    {
        Vector3 newPosition = VectorFromAngle(45);
        transform.Translate(newPosition * _speed * Time.deltaTime, Space.World);

        if (transform.position.x > 9 || transform.position.y > 9)
        {
            Destroy(this.gameObject);
        }
    }

    private void MoveRight()
    {
        transform.Translate(Vector3.right * _speed * Time.deltaTime, Space.World);
        if (transform.position.x > 9)
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

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

    public void StopLaserMovement()
    {
        _speed = 0;
    }
}
