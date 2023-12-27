using System.Collections;
using UnityEngine;

public class PortalEnemyDestroy : MonoBehaviour
{
    [SerializeField] private float lifeTime = 5f;

    void Start()
    {
        StartCoroutine(DestroyAfterTime());
    }

    IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        
        Destroy(gameObject);
    }
}
