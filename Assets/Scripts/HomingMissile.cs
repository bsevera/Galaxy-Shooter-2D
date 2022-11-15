using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingMissile : MonoBehaviour
{
    private GameObject[] _enemies;
    private Transform _targetEnemy;
    private float _distanceToEnemy;
    private float _closestEnemy;
    private Quaternion _targetEnemyRotation;

    [SerializeField]
    private float _speed = 6.0f;

    [SerializeField]
    private float _timeToLive = 20.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimeToLive());
    }

    // Update is called once per frame
    void Update()
    {
        //get all enemies on the playing field
        _enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (_enemies.Length > 0)
        {
            SetEnemyTarget();
        }
    }

    private void SetEnemyTarget()
    {
        Vector3 missilePosition = transform.position;
        Vector3 enemyPosition;

        foreach (GameObject enemy in _enemies)
        {
            if (_targetEnemy == null)
            {
                _targetEnemy = enemy.transform;
                enemyPosition = _targetEnemy.position;
                _targetEnemyRotation = Quaternion.LookRotation(transform.forward, (enemyPosition - missilePosition));
                _distanceToEnemy = Vector3.Distance(missilePosition, enemyPosition);
                _closestEnemy = _distanceToEnemy;
            }
            else
            {
                enemyPosition = enemy.transform.position;
                _targetEnemyRotation = Quaternion.LookRotation(transform.forward, (enemyPosition - missilePosition));
                _distanceToEnemy = Vector3.Distance(missilePosition, enemyPosition);
                if (_distanceToEnemy < _closestEnemy)
                {
                    _targetEnemy = enemy.transform;
                    _closestEnemy = _distanceToEnemy;
                }
            }
        }
        
        float step = _speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, _targetEnemy.position, step);
        transform.rotation = Quaternion.Slerp(transform.rotation, _targetEnemyRotation, _speed * Time.deltaTime);
    }

    IEnumerator TimeToLive()
    {
        yield return new WaitForSeconds(_timeToLive);
        Destroy(this.gameObject);
    }

    public void StopMovement()
    {
        _speed = 0;
    }
}
