using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossLaser : MonoBehaviour
{
    [SerializeField]
    private BossLaserLocation _laserLocation;

    private float _timeToLive = 20.0f;
    private float _speed = 10.0f;
    private float _leftLaserMaxValue = 0.24f;
    private float _leftLaserMinValue = -0.3826835f; //-45;
    private float _rightLaserMaxValue = 0.3826835f;
    private float _rightLaserMinValue = -0.24f;

    private string _direction = "";
    private bool _laserExtended = false;
    private bool _laserIsShrinking = false;
    private AudioSource _audioSource;
    private Boss _boss;

    // Start is called before the first frame update
    void Start()
    {
        GetAudioSourceReference();
        GetBossReference();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    #region StartUp References
    private void GetAudioSourceReference()
    {        
        _audioSource = GetComponent<AudioSource>();
        if (_audioSource == null)
        {
            Debug.LogError("Boss :: Audio Source is Null");
        }
    }

    private void GetBossReference()
    {
        GameObject boss = GameObject.FindGameObjectWithTag("Enemy");
        _boss = boss.GetComponent<Boss>();

        if (_boss == null)
        {
            Debug.LogError("BossLaser :: Unable to find Boss game object");
        }
    }

    #endregion

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

    }

    private void CalculateMovement()
    {
        //left will rotate on the z axis from -45 to 30 and back then retract
        if (_laserLocation == BossLaserLocation.Left && _laserExtended)
        {
            PauseAudioClip();
            MoveLeftLaser();            
        }

        if (_laserLocation == BossLaserLocation.Right && _laserExtended)        
        {
            PauseAudioClip();
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
                ResumeAudioClip();

                if(_laserIsShrinking == false)
                {
                    StartCoroutine(ShrinkLaser());
                }
            }
        }
    }

    private void MoveRightLaser()
    {
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
                ResumeAudioClip();
                if (_laserIsShrinking == false)
                {
                    StartCoroutine(ShrinkLaser());
                }
            }
        }
    }

    private void PauseAudioClip()
    {
        _audioSource.Pause();        
    }

    private void ResumeAudioClip()
    {
        _audioSource.Play();
    }

    private void StopAudioClip()
    {
        _audioSource.Stop();
    }

    private void SetLeftPosition()
    {
        transform.position = new Vector3(-1.7f, 2f, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, -45));        
    }

    private void SetRightPosition()
    {
        transform.position = new Vector3(1.95f, 2.1f, 0);
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, 45));
    }

    IEnumerator StretchLaser()
    {
        float y_scale = 0.2f;

        while(gameObject.transform.localScale.y < 2.5)
        {
            //y_scale += 0.2f;
            y_scale += 0.125f;

            transform.localScale = new Vector3(0.2f, y_scale, 0.2f);

            yield return new WaitForSeconds(0.3f);            
        }

        if (_laserLocation == BossLaserLocation.Left)
        {
            transform.position = new Vector3(-1.55f, 2.2f, 0);
        }
        else
        {
            transform.position = new Vector3(1.8f, 2.25f, 0);
        }
        _laserExtended = true;
    }

    IEnumerator ShrinkLaser()
    {
        float y_scale = 2.5f;
        _laserIsShrinking = true;

        while (gameObject.transform.localScale.y > 0.2f)
        {
            //y_scale -= 0.2f;
            y_scale -= 0.12f;

            transform.localScale = new Vector3(0.2f, y_scale, 0.2f);

            yield return new WaitForSeconds(0.3f);
        }

        StopAudioClip();

        _boss.LaserPoweredDown();

        Destroy(this.gameObject);
    }


}
