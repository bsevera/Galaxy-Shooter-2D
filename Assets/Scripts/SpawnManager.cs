using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;

    [SerializeField]
    private float _enemySpawnRate = 5.0f;
    private WaitForSeconds _enemySpawnRateSeconds;
    
    private float _powerupSpawnRate;

    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private GameObject[] _powerups;

    private bool _stopSpawning = false;

    public void Awake()
    {

        _enemySpawnRateSeconds = new WaitForSeconds(_enemySpawnRate);
    }

    public void StartSpawning()
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
        //wait 3 seconds before starting the spawn
        yield return new WaitForSeconds(3.0f);

        while (_stopSpawning == false)
        {
            //instantiate enemy object
            Vector3 enemyStartingPosition = new Vector3(Random.Range(-8.0f, 8.0f), 8, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, enemyStartingPosition, Quaternion.identity);
            
            newEnemy.transform.parent = _enemyContainer.transform;

            //wait for 5 seconds
            yield return _enemySpawnRateSeconds;

        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        //int powerUpToSpawn = Random.Range(0, powerups.Length);
        int randomValue = -1;
        int powerUpToSpawn = -1;

        while (_stopSpawning == false)
        {
            //create a wider range of numbers, so there is more variety in the powerups displayed
            randomValue = Random.Range(0, _powerups.Length * 100);
            powerUpToSpawn = randomValue % _powerups.Length;

            Vector3 powerUpStartingPosition = new Vector3(Random.Range(-8.0f, 8.0f), 8, 0);
            GameObject newPowerup = Instantiate(_powerups[powerUpToSpawn], powerUpStartingPosition, Quaternion.identity);            

            _powerupSpawnRate = Random.Range(3.0f, 7.0f);
            
            yield return new WaitForSeconds(_powerupSpawnRate);
        }
    }
}
