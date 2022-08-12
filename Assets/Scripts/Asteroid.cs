using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotationSpeed = 20.0f;

    [SerializeField]
    private GameObject _explosionPrefab;
    
    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.Find("Spawn_Manager").GetComponent< SpawnManager > ();
        if (_spawnManager == null)
        {
            Debug.LogError("SpawnManager is NULL");
        }
    }


    // Update is called once per frame
    void Update()
    {
        //rotate asteroid on the Z axis
        transform.Rotate(Vector3.forward * _rotationSpeed * Time.deltaTime);        

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Laser")
        {
            if (other.transform.parent != null)
            {
                //triple shot - destroy parent
                Destroy(other.transform.parent.gameObject);
            }
            else
            {
                //standard laser
                Destroy(other.gameObject);
            }
            
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);

            //start spawning enemies
            _spawnManager.StartSpawning();

            //destroy the asteroid
            Destroy(this.gameObject, 0.25f);

        }
    }

}
