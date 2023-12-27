using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource bombExplosionSource;
    [SerializeField] private AudioClip bombExplosionClip;

    public void PlayBombExplosion()
    {
        bombExplosionSource.PlayOneShot(bombExplosionClip);
    }
}
