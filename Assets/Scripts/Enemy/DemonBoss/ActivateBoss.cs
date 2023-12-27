using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateBoss : MonoBehaviour
{
    [SerializeField] private BossFightManager manager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        manager.BoundariesForThePlayer();
        gameObject.SetActive(false);
    }
}
