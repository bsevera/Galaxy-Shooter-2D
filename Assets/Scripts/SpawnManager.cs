using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private float _enemySpawnRate = 5.0f;

    [SerializeField]
    private GameObject _tripleShotPowerupPrefab;

    //[SerializeField]
    private float _powerupSpawnRate;

    [SerializeField]
    private GameObject _enemyContainer;

    private bool _stopSpawning = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemyRoutine());
        StartCoroutine(SpawnPowerupRoutine());
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    IEnumerator SpawnEnemyRoutine()
    {
        while (_stopSpawning == false)
        {
            //instantiate enemy object
            Vector3 enemyStartingPosition = new Vector3(Random.Range(-8.0f, 8.0f), 8, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, enemyStartingPosition, Quaternion.identity);
            
            newEnemy.transform.parent = _enemyContainer.transform;

            //wait for 5 seconds
            yield return new WaitForSeconds(_enemySpawnRate);

        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        while (_stopSpawning == false)
        {
            Vector3 powerUpStartingPosition = new Vector3(Random.Range(-8.0f, 8.0f), 8, 0);
            GameObject newPowerup = Instantiate(_tripleShotPowerupPrefab, powerUpStartingPosition, Quaternion.identity);

            _powerupSpawnRate = Random.Range(3.0f, 7.0f);
            
            yield return new WaitForSeconds(_powerupSpawnRate);
        }
    }
}
