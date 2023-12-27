using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEyeAudioManager : MonoBehaviour
{

    [SerializeField] private AudioSource wingsAudioSource;
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AudioClip wingClip;
    [SerializeField] private AudioClip attackClip;
    // Start is called before the first frame update
    public void PlayWingStep()
    {
        wingsAudioSource.PlayOneShot(wingClip);
    }


    public void PlayAttack()
    {
        attackAudioSource.PlayOneShot(attackClip);
    }
}
