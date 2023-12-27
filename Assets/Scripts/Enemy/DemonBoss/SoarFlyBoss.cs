using System.Collections;
using UnityEngine;
using DG.Tweening;

public class SoarFlyBoss : MonoBehaviour
{
    [SerializeField] private float soarHeight = 5f;
    [SerializeField] private float soarSpeed = 2f;
    private void Start()
    {
        StartCoroutine(SoarRoutine());
    }
    private IEnumerator SoarRoutine()
    {
        while (true)
        {
            yield return transform.DOMoveY(transform.position.y + soarHeight, soarSpeed)
                .SetEase(Ease.Linear).WaitForCompletion();

            yield return transform.DOMoveY(transform.position.y - soarHeight, soarSpeed)
                .SetEase(Ease.Linear).WaitForCompletion();

            yield return null;
        }
    }
}
