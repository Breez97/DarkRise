using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private float lifeTime;
    [SerializeField] private int countEnemies;
    [SerializeField] private float spawnInterval;
    private PlayerController playerController;
    private Transform waypointA;
    private Transform waypointB;
    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");
        if (playerObject != null)
        {
            playerController = playerObject.GetComponent<PlayerController>();
        }
        EnemyDemonBoss enemyDemonBoss = FindObjectOfType<EnemyDemonBoss>();
        if (enemyDemonBoss != null)
        {
            waypointA = enemyDemonBoss.WaypointA;
            waypointB = enemyDemonBoss.WaypointB;
        }
        StartCoroutine(SpawnEnemies());
        Destroy(gameObject, lifeTime);
    }

    IEnumerator SpawnEnemies()
    {
        for (int i = 0; i < countEnemies; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
            enemy.GetComponent<Enemy>().SetPlayerController(playerController);
            enemy.GetComponent<Enemy>().SetPoints(waypointA, waypointB);
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
