using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager Instance { get; private set; }
    float shakeTimer = 0;
    CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin;
    [SerializeField]
    CinemachineTargetGroup targetGroup;

    private void Awake()
    {
        Instance = this;
        cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    // The coroutine that shakes the camera
    public void ShakeScreen(float duration, float magnitude)
    {
        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = magnitude;
        shakeTimer = duration;
    }

    private void Update()
    {
        if (shakeTimer > 0) 
        {
            shakeTimer -= Time.deltaTime;
            if (shakeTimer <= 0) 
            {
                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 0;
            }
        }
    }

    public void AddToTargetGroup(Transform objectTransform) 
    {
        targetGroup.AddMember(objectTransform, 1.0f, 1.0f);
    }

    public void RemoveFromTargetGroup(Transform objectTransform) 
    {
        targetGroup.RemoveMember(objectTransform);
    }
}