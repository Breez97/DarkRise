using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource walkAudioSource;
    [SerializeField] private AudioSource jumpFallAudioSource;
    [SerializeField] private AudioSource attackAudioSource;
    [SerializeField] private AudioSource hitSource;
    [SerializeField] private AudioClip walkClip;
    [SerializeField] private AudioClip runClip;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip deathClip;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private AudioClip rollClip;
    [SerializeField] private AudioClip fallClip;
    [SerializeField] private AudioClip shieldClip;
    [SerializeField] private AudioClip[] attackMissClips;
    [SerializeField] private AudioClip[] attackHitClips;
    [SerializeField] private AudioClip fireSpellClip;
    [SerializeField] private AudioClip windSpellClip;
    private bool isJump = false;
    // Start is called before the first frame update
    public void PlayWalkStep()
    {
        walkAudioSource.PlayOneShot(walkClip);
    }
    public void PlaRunStep()
    {
        walkAudioSource.PlayOneShot(runClip);
    }
    public void PlayJumpClip()
    {
        if (!isJump)
        {
            isJump = true;
            jumpFallAudioSource.PlayOneShot(jumpClip);
        }
        
    }
    public void SetNotJump()
    {
        isJump = false; 
    }

    public void PlayRollClip()
    {
        walkAudioSource?.PlayOneShot(rollClip); 
    }
    public void PlayFallClip()
    {
        jumpFallAudioSource.PlayOneShot(fallClip);
    }
    public void PlayAttackMiss()
    {
        attackAudioSource.PlayOneShot(attackMissClips[Random.Range(0, attackMissClips.Length)]);
    }
    public void PlayAttackHit()
    {
        attackAudioSource.PlayOneShot(attackHitClips[Random.Range(0, attackHitClips.Length)]);
    }

    public void PlayFireSpellAttack()
    {
        attackAudioSource.PlayOneShot(fireSpellClip);
    }

    public void PlaySaintAttack()
    {
        attackAudioSource.PlayOneShot(shieldClip);
    }
    public void PlayWindSpellAttack()
    {
        attackAudioSource.PlayOneShot(windSpellClip);
    }

    public void PlayDeath()
    {
        walkAudioSource.PlayOneShot(deathClip);
    }
    public void PlayHit()
    {
        hitSource.PlayOneShot(hitClip);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
