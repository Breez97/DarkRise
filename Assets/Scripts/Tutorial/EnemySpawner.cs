using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private Transform enemyPosition;
    [SerializeField] private GameObject enemyPrefab;
    private GameObject currentEnemy;
    private Damageable enemyDamageable;
    private bool isWaitingToSpawn = false;

    private void Start()
    {
        SpawnEnemy();
    }

    private void Update()
    {
        if (currentEnemy != null)
        {
            enemyDamageable = currentEnemy.GetComponent<Damageable>();

            if (enemyDamageable != null && !enemyDamageable.IsAlive && !isWaitingToSpawn)
            {
                StartCoroutine(WaitToSpawnEnemy());
            }
        }
    }

    private void SpawnEnemy()
    {
        currentEnemy = Instantiate(enemyPrefab, enemyPosition.position, Quaternion.Euler(0f, 180f, 0f));
    }

    private IEnumerator WaitToSpawnEnemy()
    {
        isWaitingToSpawn = true;
        yield return new WaitForSeconds(2f);
        SpawnEnemy();
        isWaitingToSpawn = false;
    }
}
