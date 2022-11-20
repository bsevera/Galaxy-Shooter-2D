using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    [SerializeField]
    private BossLaserLocation _laserLocation;

    private float _timeToLive = 20.0f;
    private float _speed = 7.0f;
    private float _leftLaserMaxValue = 0.24f;
    private float _leftLaserMinValue = -0.3826835f; //-45;
    private float _rightLaserMaxValue = 0.3826835f;
    private float _rightLaserMinValue = -0.24f;

    private string _direction = "";

    // Start is called before the first frame update
    void Start()
    {
        //if (_laserLocation == BossLaserLocation.Left)
        //{
        //    SetLeftPosition();
        //    _direction = "forward";
        //}
        //else
        //{
        //    SetRightPosition();
        //    _direction = "reverse";
        //}

        //StartCoroutine(StretchLaser());
        //StartCoroutine(Cooldown());
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    public void SetLocation(BossLaserLocation location)
    {
        _laserLocation = location;

        if (_laserLocation == BossLaserLocation.Left)
        {
            SetLeftPosition();
            _direction = "forward";
        }
        else
        {
            SetRightPosition();
            _direction = "reverse";
        }

        StartCoroutine(StretchLaser());
        StartCoroutine(Cooldown());

    }

    private void CalculateMovement()
    {
        //left will rotate on the z axis from -45 to 30 and back then retract
        if (_laserLocation == BossLaserLocation.Left)
        {
            MoveLeftLaser();
            
        }
        else
        {
            MoveRightLaser();
        }
    }

    private void MoveLeftLaser()
    {
        if (_direction == "forward")
        {
            if (transform.rotation.z < _leftLaserMaxValue)            
            {
                transform.Rotate(Vector3.forward * _speed * Time.deltaTime);
            }
            else
            {
                _direction = "reverse";
            }
        }
        else
        {
            if (transform.rotation.z > _leftLaserMinValue)
            {
                transform.Rotate(Vector3.back * _speed * Time.deltaTime);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void MoveRightLaser()
    {
        Debug.Log("MoveRightLaser");
        Debug.Log("Direction = " + _direction);
        Debug.Log("Rotation.z = " + transform.rotation.z);

        if (_direction == "reverse")
        {            
            if (transform.rotation.z > _rightLaserMinValue)
            {
                transform.Rotate(Vector3.back * _speed * Time.deltaTime);
            }
            else
            {
                _direction = "forward";
            }
        }
        else
        {
            if (transform.rotation.z < _rightLaserMaxValue)
            {
                transform.Rotate(Vector3.forward * _speed * Time.deltaTime);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void SetLeftPosition()
    {
        transform.position = new Vector3(-1.7f, 2f, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));        
    }

    private void SetRightPosition()
    {
        Debug.Log("Set Right Position");
        transform.position = new Vector3(1.95f, 2.1f, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));
    }

    IEnumerator StretchLaser()
    {
        Debug.Log("Stretch Laser");
        float y_scale = 0.2f;        

        while(gameObject.transform.localScale.y < 2.5)
        {
            y_scale += 0.2f;

            transform.localScale = new Vector3(0.2f, y_scale, 0.2f);            

            yield return new WaitForSeconds(2.0f);
        }

        if (_laserLocation == BossLaserLocation.Left)
        {
            transform.position = new Vector3(-1.55f, 2.2f, 0);
        }
        else
        {
            transform.position = new Vector3(1.8f, 2.25f, 0);
        }

    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(_timeToLive);
        Destroy(this.gameObject);

    }


}
