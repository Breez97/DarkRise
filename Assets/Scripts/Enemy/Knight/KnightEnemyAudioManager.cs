using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightEnemyAudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip attackClip;
    public void PlayWalkStep()
    {
        walkAudioSource.PlayOneShot(walkClip);
    }


    public void PlayAttack()
    {
        attackAudioSource.PlayOneShot(attackClip);
    }
}
