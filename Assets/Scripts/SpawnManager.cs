using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyBluePrefab;
    [SerializeField]
    private GameObject _enemyAggressorPrefab;
    [SerializeField]
    private GameObject _enemyPinkPrefab;
    [SerializeField]
    private GameObject _enemyBossPrefab;

    [SerializeField]
    private float _enemySpawnRate = 5.4f; //was 5.0f
    private WaitForSeconds _enemySpawnRateSeconds;
    
    private float _powerupSpawnRate;

    [SerializeField]
    private GameObject _enemyContainer;

    [SerializeField]
    private GameObject[] _powerups;

    private bool _stopSpawning = false;

    private int[] _powerupRarityMonitor;

    private UIManager _UIManager;

    private int[] _enemiesPerWave;
    private int _enemiesSpawnedThisWave = 0;
    private int _enemiesKilledThisWave = 0;
    private int _currentWave = 0;
    private Coroutine _co;

    private int _maxWaves = 5;

    public void Awake()
    {
        _enemySpawnRateSeconds = new WaitForSeconds(_enemySpawnRate);
        InitializeEnemiesPerWave();

        InitializePowerupRarityMonitor();
    }

    public void Start()
    {
        //get the UI Manager
        _UIManager = GameObject.Find("Canvas").GetComponent<UIManager>();

        if (_UIManager == null)
        {
            Debug.LogError("SpawnManager :: UI Manager is null");
        }
    }

    public void Update()
    {
        //if wave complete, stop powerup spawns, start new wave
        if (_currentWave > 0) 
        {
            _UIManager.UpdateKills(_enemiesKilledThisWave, _enemiesPerWave[_currentWave]);

            if (_enemiesKilledThisWave == _enemiesPerWave[_currentWave])
            {
                StopCoroutine(_co);
                _currentWave += 1;
                SpawnNextWave();
            }
        }
    }

    private void InitializeEnemiesPerWave()
    {        
        _enemiesPerWave = new int[_maxWaves + 1];

        //array[0] will equal 0 to make it easier to keep track of which wave we're on
        for (int i = 0; i < _maxWaves; i++)
        {
            _enemiesPerWave[i] = i * 5;
        }

        //set value for final (boss) wave
        _enemiesPerWave[_maxWaves] = 1;
    }

    private void InitializePowerupRarityMonitor()
    {
        _powerupRarityMonitor = new int[_powerups.Length];

        for (int i = 0; i < _powerupRarityMonitor.Length; i++)
        {
            _powerupRarityMonitor[i] = 0;
        }
    }

    public void StartSpawning()
    {
        _currentWave += 1;
        SpawnNextWave();
    }


    private void SpawnNextWave()
    {
        _UIManager.DisplayWaveText(_currentWave);

        _enemiesKilledThisWave = 0;
        _enemiesSpawnedThisWave = 0;
        _enemySpawnRate -= .4f;

        if (_currentWave <= _maxWaves)
        {
            StartCoroutine(SpawnEnemyRoutine(_enemiesPerWave[_currentWave]));
        }
    }

    public void OnEnemyKilled()
    {
        _enemiesKilledThisWave += 1;
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    private bool IsLastWave()
    {
        return (_currentWave == _maxWaves);
    }

    private void SpawnNewEnemy()
    {
        int enemyToSpawn = -1;

        if (IsLastWave())
        {
            //spawn boss
            enemyToSpawn = 4;
        }
        else
        {
            enemyToSpawn = Random.Range(0, _currentWave);
        }

        GameObject newEnemy = null;
        Vector3 enemyStartingPosition;

        enemyStartingPosition = new Vector3(Random.Range(-8.0f, 8.0f), 8, 0);

        switch (enemyToSpawn)
        {
            case 0:
                newEnemy = Instantiate(_enemyPrefab, enemyStartingPosition, Quaternion.identity);
                break;
            case 1:
                newEnemy = Instantiate(_enemyPinkPrefab, enemyStartingPosition, Quaternion.identity);
                break;
            case 2:
                newEnemy = Instantiate(_enemyAggressorPrefab, enemyStartingPosition, Quaternion.identity);
                break;
            case 3:
                newEnemy = Instantiate(_enemyBluePrefab, enemyStartingPosition, Quaternion.identity);
                break;
            case 4:
                enemyStartingPosition = new Vector3(0, 11, 0);
                newEnemy = Instantiate(_enemyBossPrefab, enemyStartingPosition, Quaternion.identity);
                break;
        }

        newEnemy.transform.parent = _enemyContainer.transform;

    }

    IEnumerator SpawnEnemyRoutine(int enemiesToSpawn)
    {
        //wait 3 seconds before starting the spawn
        yield return new WaitForSeconds(3.0f);

        _co = StartCoroutine(SpawnPowerupRoutine());

        while (_stopSpawning == false)
        {
            if (_enemiesSpawnedThisWave < enemiesToSpawn)
            {
                _enemiesSpawnedThisWave += 1;
                SpawnNewEnemy();                

                //wait to spawn next enemy
                yield return _enemySpawnRateSeconds;
            }
            else
            {
                //stop the coroutine, all enemies have been rendered
                yield break;
            }
        }
    }

    IEnumerator SpawnPowerupRoutine()
    {
        yield return new WaitForSeconds(3.0f);

        int powerUpToSpawn = -1;

        while (_stopSpawning == false)
        {
            //create a wider range of numbers, so there is more variety in the powerups displayed
            powerUpToSpawn = Random.Range(0, _powerups.Length);            

            if (IsReadyToSpawn(powerUpToSpawn, _powerups[powerUpToSpawn].GetComponent<Powerup>().Rarity))
            {
                //instantiate the new powerup
                Vector3 powerUpStartingPosition = new Vector3(Random.Range(-8.0f, 8.0f), 8, 0);
                GameObject newPowerup = Instantiate(_powerups[powerUpToSpawn], powerUpStartingPosition, Quaternion.identity);

                _powerupSpawnRate = Random.Range(3.0f, 7.0f);

                //reset the value of the rarity monitor back to 0
                _powerupRarityMonitor[powerUpToSpawn] = 0;

                yield return new WaitForSeconds(_powerupSpawnRate);

            }
            else
            {
                //increase the count to simulate the rareness
                _powerupRarityMonitor[powerUpToSpawn] += 1;

                //wait before attempting to spawn another powerup
                yield return new WaitForSeconds(1.5f);
            }
        }
    }

    private bool IsReadyToSpawn(int powerupID, PowerupRarity rarity)
    {
        if (_powerupRarityMonitor[powerupID] == (int)rarity)
        { 
            return true; 
        }
        else
        {
            return false;
        }
    }

}

