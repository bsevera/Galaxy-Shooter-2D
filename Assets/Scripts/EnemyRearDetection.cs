using UnityEngine;

public class EnemyRearDetection : MonoBehaviour
{
    private bool _playerDetectedBehind;
    public bool PlayerDetectedBehind
    {
        get { return _playerDetectedBehind; }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _playerDetectedBehind = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            _playerDetectedBehind = false;
        }
    }

 }
