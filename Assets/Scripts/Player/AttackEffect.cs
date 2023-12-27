using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using Cinemachine.PostFX;

public class AttackEffect : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private CinemachineBasicMultiChannelPerlin multiChannelPerlin;
    [SerializeField] private float shakeIntensity = 1f;
    [SerializeField] private float shakeTime = 0.2f;
    [SerializeField] private float timer;
    [SerializeField] CinemachineVolumeSettings volumeSettings;
    [SerializeField] ChromaticAberration chromaticAberration;
    [SerializeField] Bloom bloom;
    [SerializeField] private float  bloomDefaultIntensity;

    private void ShakeCamera()
    {
        multiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        multiChannelPerlin.m_AmplitudeGain = shakeIntensity;
        timer = shakeTime;
    }

    private void StopShake()
    {
        multiChannelPerlin = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        multiChannelPerlin.m_AmplitudeGain = 0f;
        timer = 0;
    }
    void Start()
    {
        StopShake();
    }

    void Update()
    {
        
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if(timer <= 0)
            {
                StopShake();
                if (volumeSettings.m_Profile.TryGet<ChromaticAberration>(out chromaticAberration))
                {
                    chromaticAberration.active = false;
                }
                if(volumeSettings.m_Profile.TryGet(out bloom))
                {
                    bloom.intensity.Override(bloomDefaultIntensity);
                }
            }
        }
    }

    public void OnAttackEffect()
    {
        if(volumeSettings.m_Profile.TryGet<ChromaticAberration>(out chromaticAberration))
        {
            chromaticAberration.active = true;
        }
        if (volumeSettings.m_Profile.TryGet(out bloom))
        {
            bloomDefaultIntensity = bloom.intensity.value;
            bloom.intensity.Override(7f);

        }
        if (timer <= 0)
        {
            ShakeCamera();
        }

    }
}
