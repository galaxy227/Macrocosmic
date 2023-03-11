using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public enum VolumeType
{
    Standard,
    Background
}

public class PostProcessingController : MonoBehaviour
{
    public DelayType DelayType;

    public Camera MainCamera;
    private UniversalAdditionalCameraData MainUAC;
    public Camera BackgroundCamera;
    private UniversalAdditionalCameraData BackgroundUAC;

    private Volume volume;
    private DepthOfField depthOfField;
    private Bloom bloom;

    private float secondsToFade;
    private bool isCoroutineRunning;

    public static PostProcessingController Instance
    {
        get { return instance; }
    }
    private static PostProcessingController instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        SetComponents();
    }
    private void Start()
    {
        SetVolume(VolumeType.Standard, false);
    }

    public void SetVolume(VolumeType volumeType, bool isFade)
    {
        // Stop previous coroutine
        if (isCoroutineRunning)
        {
            StopFade();
        }

        // Set volume
        if (isFade)
        {
            // Start new coroutine
            StartCoroutine(HandleFade(volumeType));
            isCoroutineRunning = true;
        }
        else
        {
            SetVolumeData(volumeType, 1f);
        }
    }
    private IEnumerator HandleFade(VolumeType volumeType)
    {
        ResetVolumeData();

        secondsToFade = DelayHelper.GetDelayTime(DelayType);
        float timer = 0;

        do
        {
            timer += Time.unscaledDeltaTime;
            float multiplier = Mathf.InverseLerp(0, secondsToFade, timer);

            SetVolumeData(volumeType, multiplier);

            yield return null;

        } while (timer < secondsToFade);

        StopFade();
    }
    private void StopFade()
    {
        StopCoroutine("HandleFade");
        isCoroutineRunning = false;
    }
    private void SetVolumeData(VolumeType volumeType, float multiplier)
    {
        if (volumeType == VolumeType.Standard)
        {
            MainUAC.renderPostProcessing = true;

            // DepthOfField
            depthOfField.active = false;

            // Bloom
            bloom.active = false;
        }
        else if (volumeType == VolumeType.Background)
        {
            MainUAC.renderPostProcessing = false;

            // DepthOfField
            depthOfField.active = true;
            depthOfField.mode.value = DepthOfFieldMode.Bokeh;
            depthOfField.focusDistance.value = 10 * multiplier;
            depthOfField.focalLength.value = 150 * multiplier; // 35
            depthOfField.bladeCurvature.value = 0.75f * multiplier;

            // Bloom
            bloom.active = true;
            bloom.threshold.value = 0.15f;
            bloom.intensity.value = 1.5f * multiplier;
        }
    }
    private void ResetVolumeData()
    {
        // DepthOfField
        depthOfField.mode.value = DepthOfFieldMode.Off;
        depthOfField.focusDistance.value = 0;
        depthOfField.focalLength.value = 0;
        depthOfField.bladeCurvature.value = 0;

        // Bloom
        bloom.threshold.value = 0;
        bloom.intensity.value = 0;
    }

    // Utility
    private void SetComponents()
    {
        if (TryGetComponent(out Volume outVolume))
        {
            volume = outVolume;
        }

        if (volume.profile.TryGet(out DepthOfField outDepthOfField))
        {
            depthOfField = outDepthOfField;
        }
        if (volume.profile.TryGet(out Bloom outBloom))
        {
            bloom = outBloom;
        }

        MainUAC = MainCamera.GetComponent<UniversalAdditionalCameraData>();
        BackgroundUAC = BackgroundCamera.GetComponent<UniversalAdditionalCameraData>();
    }
}