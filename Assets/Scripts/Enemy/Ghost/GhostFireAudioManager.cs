using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostFireAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource fireExplosionSource;
    [SerializeField] private AudioClip fireExplosionClip;

    public void PlayFireExplosion()
    {
        fireExplosionSource.PlayOneShot(fireExplosionClip);
    }
}
