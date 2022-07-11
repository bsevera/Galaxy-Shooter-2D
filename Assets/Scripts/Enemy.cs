using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _speed = 4.0f;

    private float _bottomOfScreen = -5.4f;
    private float _topOfScreen = 6.5f;
    private float _minX = -11.0f;
    private float _maxX = 11.0f;

    // Start is called before the first frame update
    void Start()
    {
        //position object at the top of the screen
        SetStartPosition();
    }

    // Update is called once per frame
    void Update()
    {

        if (transform.position.y > _bottomOfScreen)
        {
            //move down 4 meters per second
            transform.Translate(Vector3.down * _speed * Time.deltaTime);
        }
        else
        {
            //if object moves off of the bottom of screen, respawn it at the top with a new random x position
            SetStartPosition();
        }
    }

    private void SetStartPosition()
    {
        float randomX = Random.Range(_minX, _maxX);

        transform.position = new Vector3(randomX, _topOfScreen, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            //get player object
            Player player = other.transform.GetComponent<Player>();

            //damage the player
            if (player != null)
            {
                player.Damage();
            }

            //destroy us
            Destroy(this.gameObject);
        }

        if (other.tag == "Laser")
        {
            //destroy laser
            Destroy(other.gameObject);

            //destroy us
            Destroy(this.gameObject);
        }                
    }

}
