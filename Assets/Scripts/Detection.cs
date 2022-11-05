using UnityEngine;

public class Detection : MonoBehaviour
{
    private bool _playerDetected;
    public bool PlayerDetected
    {
        get { return _playerDetected; }
    }
    
    private bool _laserDetected;
    public bool LaserDetected
    {
        get { return _laserDetected; }
    }

    private bool _powerupDetected;
    public bool PowerupDetected
    {
        get { return _powerupDetected; }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _playerDetected = true;
        }

        if (other.tag == "Laser")
        {
            _laserDetected = true;
        }

        if (other.tag == "Powerup")
        {
            _powerupDetected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _playerDetected = false;
        }

        if (other.tag == "Laser")
        {
            _laserDetected = false;
        }

        if (other.tag == "Powerup")
        {
            _powerupDetected = false;
        }
    }

}
