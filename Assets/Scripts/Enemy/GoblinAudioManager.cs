using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoblinAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip[] attackClips;
    [SerializeField] private AudioClip[] throwBomb;
    // Start is called before the first frame update
    public void PlayWalkStep()
    {
        walkAudioSource.PlayOneShot(walkClip);
    }

    public void PlayThrowBomb()
    {
        int randomLaughIndex = Random.Range(0, throwBomb.Length + 1);
        if (randomLaughIndex < throwBomb.Length)
        {
            attackAudioSource.PlayOneShot(throwBomb[Random.Range(0, throwBomb.Length)]);
        }
    }

    public void PlayAttack()
    {
        attackAudioSource.PlayOneShot(attackClips[Random.Range(0, attackClips.Length)]);
    }

}
