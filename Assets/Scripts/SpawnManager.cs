using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;

    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnRoutine());
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }


    IEnumerator SpawnEnemyObject()
    {
        while (true)
        {
            //instantiate enemy object
            Vector3 enemyStartingPosition = new Vector3(Random.Range(-8.0f, 8.0f), 8, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, enemyStartingPosition, Quaternion.identity);

            newEnemy.transform.parent = _enemyContainer.transform;

            yield return new WaitForSeconds(5.0f);
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (_stopSpawning == false)
        {
            //instantiate enemy object
            Vector3 enemyStartingPosition = new Vector3(Random.Range(-8.0f, 8.0f), 8, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, enemyStartingPosition, Quaternion.identity);
            
            newEnemy.transform.parent = _enemyContainer.transform;

            //wait for 5 seconds
            yield return new WaitForSeconds(5.0f);

        }
    }
}
