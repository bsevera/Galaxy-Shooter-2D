using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 8.0f;


    // Update is called once per frame
    void Update()
    {

        //move laser up
        transform.Translate(Vector3.up * _speed * Time.deltaTime);

        //if position on the Y is greater than 8, destroy the object
        if (transform.position.y > 8f)
        {
            Destroy(this.gameObject);
        }
    }
}
