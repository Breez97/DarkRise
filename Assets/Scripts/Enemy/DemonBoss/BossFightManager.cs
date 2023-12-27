using System.Collections;
using UnityEngine;

public class BossFightManager : MonoBehaviour
{
    [SerializeField] private GameObject[] borders;
    [SerializeField] private int maxEnemiesSpawned = 2;
    [SerializeField] private float interval = 0.003f;
    private bool canCreate = true;
    private bool isStartingFight = false;
    private bool isEnemyPortalStarted = false;

    public bool CanCreate { get => canCreate; set => canCreate = value; }
    public bool IsStartingFight { get => isStartingFight; set => isStartingFight = value; }

    private void Awake()
    {
        foreach (var border in borders)
        {
            border.SetActive(false);
        }
    }

    private void Update()
    {
        if (isStartingFight && !isEnemyPortalStarted)
        {
            StartCoroutine(CheckEnemyPortalsPeriodically(interval));
            isEnemyPortalStarted = true;
        }
    }

    private IEnumerator CheckEnemyPortalsPeriodically(float interval)
    {
        while (isStartingFight)
        {
            GameObject[] enemyPortals = GameObject.FindGameObjectsWithTag("portalEntity");
            CanCreate = enemyPortals.Length <= maxEnemiesSpawned;

            Debug.Log("Number of enemy portals: " + enemyPortals.Length + " " + canCreate);

            yield return new WaitForSeconds(interval);
        }
    }

    public void IsBossDeath()
    {
        GameObject[] enemyPortals = GameObject.FindGameObjectsWithTag("portalEntity");
        foreach (var enemy in enemyPortals)
        {
            Destroy(enemy);
        }

        foreach (var border in borders)
        {
            Destroy(border);
        }
    }

    public void BoundariesForThePlayer()
    {
        if (!IsStartingFight)
        {
            IsStartingFight = true;
            foreach (var border in borders)
            {
                border.SetActive(true);
            }
        }
    }
}
