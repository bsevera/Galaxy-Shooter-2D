using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;
    
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
            MoveUp();
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

    public void AssignEnemyLaser()
    {
        _isEnemyLaser = true;
    }

}
