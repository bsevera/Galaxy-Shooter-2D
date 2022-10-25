using UnityEngine;

public class Detection : MonoBehaviour
{
    private bool _playerDetected;

    public bool PlayerDetected
    {
        get { return _playerDetected; }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _playerDetected = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _playerDetected = false;
        }
    }

}
