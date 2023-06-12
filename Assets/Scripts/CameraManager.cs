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
    [SerializeField]
    Transform targetTracker;
    [SerializeField]
    float kickbackResetDuration = 1.0f;
    [SerializeField]
    float kickbackResetTimer = 0;
    [SerializeField]
    bool kickbackComplete = true;
    [SerializeField]
    float kickbackResetScale = 0.5f;
    [SerializeField]
    CompositeCollider2D tilemapCollider;
    [SerializeField]
    Camera mapCamera;

    private void Awake()
    {
        Instance = this;
        cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGamePlaying()) 
        {
            mapCamera.transform.position = tilemapCollider.bounds.center + Vector3Int.back * 10;
            mapCamera.orthographicSize = Mathf.Max(tilemapCollider.bounds.extents.x, tilemapCollider.bounds.extents.y);
        }
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
        if (kickbackResetTimer > 0)
        {
            kickbackResetTimer -= Time.deltaTime;
            targetTracker.localPosition -= targetTracker.localPosition * kickbackResetScale;
        }
        else if (!kickbackComplete) 
        {
            kickbackComplete = true;
            targetTracker.localPosition = Vector3Int.zero;
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

    public void Kickback(Vector2 kickbackDir) 
    {
        targetTracker.localPosition -= (Vector3)kickbackDir;
        kickbackResetTimer = kickbackResetDuration;
        kickbackComplete = false;
    }
}