using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostAudioManager : MonoBehaviour
{
    [SerializeField] AudioSource attackSource;
    [SerializeField] AudioSource appearSource;
    [SerializeField] AudioSource ghostSoundSource;
    [SerializeField] AudioClip attackClip;
    [SerializeField] AudioClip appearClip;
    [SerializeField] AudioClip ghostSoundClip;

    public void PlayAttack()
    {
        attackSource.PlayOneShot(attackClip);
    }

    public void PlayAppear()
    {
        appearSource.PlayOneShot(appearClip);   
    }

    public void PlayGhostSound()
    {
        int randomNum = Random.Range(0, 3);

        if(randomNum == 0)
        {
            ghostSoundSource.PlayOneShot(ghostSoundClip);
        }
        
    }
}
