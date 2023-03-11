using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BackgroundType
{
    Fractal,
    Gradient,
    PopUp,
}

// Handles object rendering background for UI
public class BackgroundController : MonoBehaviour
{
    [HideInInspector] public DelayType DelayType;
    // Background objects & respective materials
    public GameObject FractalObj;
    public Material FractalMat;
    public GameObject GradientObj;
    public Material GradientMat;
    public GameObject PopUpObj;
    public Material PopUpMat;

    private List<GameObject> currentObjectList = new List<GameObject>();
    private GameObject CurrentObj;
    private Material CurrentMat;
    private const float initialAlpha = 0f;
    private const float endAlpha = 1f;
    private float secondsToFade;
    private bool isCoroutineRunning;

    public static BackgroundController Instance
    {
        get { return instance; }
    }
    private static BackgroundController instance;

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
    }
    private void Start()
    {
        OnStart();
    }

    // Background
    public void SetBackground(bool isActive, BackgroundType backgroundType, DelayType delayType = DelayType.Fast)
    {
        DelayType = delayType;

        // Stop previous coroutine
        if (isCoroutineRunning)
        {
            StopFade();
        }

        if (isActive)
        {
            GameObject originalObj = CurrentObj;

            // Set Object & Material
            if (backgroundType == BackgroundType.Fractal)
            {
                CurrentObj = FractalObj;
                CurrentMat = FractalMat;

                PostProcessingController.Instance.SetVolume(VolumeType.Background, true);
            }
            else if (backgroundType == BackgroundType.Gradient)
            {
                CurrentObj = GradientObj;
                CurrentMat = GradientMat;

                PostProcessingController.Instance.SetVolume(VolumeType.Background, true);
            }
            else if (backgroundType == BackgroundType.PopUp)
            {
                CurrentObj = PopUpObj;
                CurrentMat = PopUpMat;

                PostProcessingController.Instance.SetVolume(VolumeType.Background, true);
            }

            // Handle currentObjectList
            if (currentObjectList.Contains(CurrentObj))
            {
                // Remove all objects after CurrentObject in currentObjectList
                int currentIndex = currentObjectList.IndexOf(CurrentObj);

                for (int i = currentObjectList.Count - 1; i >= 0; i--)
                {
                    if (i > currentIndex)
                    {
                        ResetBackground(currentObjectList[i]);
                        currentObjectList.RemoveAt(i);
                    }
                }
            }
            else
            {
                // Add CurrentObject to currentObjectList
                CurrentObj.GetComponent<FollowCamera>().z += currentObjectList.Count * 0.01f; // Prevent overlap of quads
                currentObjectList.Add(CurrentObj);
            }

            CurrentObj.SetActive(true);

            if (CurrentObj != originalObj)
            {
                // Start new coroutine
                StartFade();
            }
        }
        else
        {
            // Disable the argument backgroundType
            if (currentObjectList.Count >= 2) // Background to reveal after disabling backgroundType
            {
                // Reveal last background
                if (currentObjectList[currentObjectList.Count - 2] != null)
                {
                    currentObjectList[currentObjectList.Count - 2].SetActive(true);

                    CurrentObj = currentObjectList[currentObjectList.Count - 2];
                    CurrentMat = currentObjectList[currentObjectList.Count - 2].GetComponent<Material>();
                }

                // Disable individual background
                GameObject objToDisable = null;

                switch (backgroundType)
                {
                    case BackgroundType.Fractal:
                        objToDisable = FractalObj;
                        break;
                    case BackgroundType.Gradient:
                        objToDisable = GradientObj;
                        break;
                    case BackgroundType.PopUp:
                        objToDisable = PopUpObj;
                        break;
                }

                ResetBackground(objToDisable);
                currentObjectList.Remove(objToDisable);
            }
            else // No background to reveal after disabling backgroundType
            {
                DisableAllBackgrounds();
            }
        }
    }
    private void ResetBackground(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);

            if (obj.TryGetComponent(out FollowCamera followCamera))
            {
                followCamera.z = -0.45f;
            }
        }
    }
    public void DisableAllBackgrounds()
    {
        foreach (GameObject obj in currentObjectList)
        {
            ResetBackground(obj);
        }

        CurrentObj = null;
        CurrentMat = null;

        currentObjectList.Clear();

        PostProcessingController.Instance.SetVolume(VolumeType.Standard, false);
    }
    private void HideAllNonCurrentBackgrounds()
    {
        foreach (GameObject obj in currentObjectList)
        {
            if (obj != CurrentObj)
            {
                obj.SetActive(false);
            }
        }
    }

    // Fade
    private IEnumerator HandleFade()
    {
        // Reset alpha
        CurrentMat.SetFloat("_Alpha", initialAlpha);

        // Declare variables
        secondsToFade = DelayHelper.GetDelayTime(DelayType);
        bool isIncompleteFade = false;

        do
        {
            isIncompleteFade = false;

            float increment = (1f / secondsToFade) * Time.unscaledDeltaTime;

            if (CurrentMat.GetFloat("_Alpha") < endAlpha / 2f) // if less than half endAlpha
            {
                increment *= 2f; // faster
            }
            else
            {
                increment /= 2f; // slower
            }

            float newAlpha = Mathf.Clamp(CurrentMat.GetFloat("_Alpha") + increment, initialAlpha, endAlpha);

            CurrentMat.SetFloat("_Alpha", newAlpha);

            if (newAlpha < endAlpha)
            {
                isIncompleteFade = true;
            }

            yield return null;

        } while (isIncompleteFade);

        StopFade();

        HideAllNonCurrentBackgrounds();
    }
    private void StartFade()
    {
        StartCoroutine(HandleFade());
        isCoroutineRunning = true;
    }
    private void StopFade()
    {
        StopCoroutine(HandleFade());
        isCoroutineRunning = false;
    }

    // Utility
    private void OnStart()
    {
        currentObjectList.Add(FractalObj);
        currentObjectList.Add(GradientObj);
        currentObjectList.Add(PopUpObj);

        DisableAllBackgrounds();
    }
}

//// Handles object rendering background for UI
//public class BackgroundController : MonoBehaviour
//{
//    public DelayType DelayType;
//    // Background objects & respective materials
//    public GameObject FractalObj;
//    public Material FractalMat;
//    public GameObject GradientObj;
//    public Material GradientMat;

//    public List<GameObject> CurrentObjects = new List<GameObject>();
//    private GameObject CurrentObj;
//    private Material CurrentMat;
//    private const float initialAlpha = 0f;
//    private const float endAlpha = 1f;
//    private float secondsToFade;
//    private bool isCoroutineRunning;

//    public static BackgroundController Instance
//    {
//        get { return instance; }
//    }
//    private static BackgroundController instance;

//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//    private void Start()
//    {
//        OnStart();
//    }

//    // Background
//    public void SetBackground(bool isActive, BackgroundType backgroundType = BackgroundType.Fractal)
//    {
//        GameObject originalObj = CurrentObj;

//        // Stop previous coroutine
//        if (isCoroutineRunning)
//        {
//            StopFade();
//        }

//        if (isActive)
//        {
//            // Set Object & Material
//            if (backgroundType == BackgroundType.Fractal)
//            {
//                CurrentObj = FractalObj;
//                CurrentMat = FractalMat;
//            }
//            else if (backgroundType == BackgroundType.Gradient)
//            {
//                CurrentObj = GradientObj;
//                CurrentMat = GradientMat;
//            }

//            CurrentObj.SetActive(true);
//            CurrentObjects.Add(CurrentObj);

//            if (CurrentObj != originalObj)
//            {
//                // Start new coroutine
//                StartFade();
//            }
//        }
//        else
//        {
//            CloseAllBackgrounds();
//        }
//    }
//    private void CloseAllBackgrounds()
//    {
//        foreach (GameObject obj in CurrentObjects)
//        {
//            obj.SetActive(false);
//        }

//        CurrentObjects.Clear();

//        CurrentObj = null;
//        CurrentMat = null;

//        PostProcessingController.Instance.SetVolume(VolumeType.Standard, false);
//    }
//    private void CloseNonCurrentBackgrounds()
//    {
//        foreach (GameObject obj in CurrentObjects)
//        {
//            if (obj != CurrentObj)
//            {
//                obj.SetActive(false);
//            }
//        }
//    }

//    // Fade
//    private IEnumerator HandleFade()
//    {
//        // Reset alpha
//        CurrentMat.SetFloat("_Alpha", initialAlpha);

//        // Declare variables
//        secondsToFade = DelayHelper.GetDelayTime(DelayType);
//        bool isIncompleteFade = false;

//        PostProcessingController.Instance.SetVolume(VolumeType.Background, true);

//        do
//        {
//            isIncompleteFade = false;

//            float increment = (1f / secondsToFade) * Time.unscaledDeltaTime;

//            if (CurrentMat.GetFloat("_Alpha") < endAlpha / 2f) // if less than half endAlpha
//            {
//                increment *= 2f; // faster
//            }
//            else
//            {
//                increment /= 2f; // slower
//            }

//            float newAlpha = Mathf.Clamp(CurrentMat.GetFloat("_Alpha") + increment, initialAlpha, endAlpha);

//            CurrentMat.SetFloat("_Alpha", newAlpha);

//            if (newAlpha < endAlpha)
//            {
//                isIncompleteFade = true;
//            }

//            yield return null;

//        } while (isIncompleteFade);

//        StopFade();
//        CloseNonCurrentBackgrounds();
//    }
//    private void StartFade()
//    {
//        StartCoroutine(HandleFade());
//        isCoroutineRunning = true;
//    }
//    private void StopFade()
//    {
//        StopCoroutine(HandleFade());
//        isCoroutineRunning = false;
//    }

//    // Utility
//    private void OnStart()
//    {
//        // Set all background objects false
//        FractalObj.gameObject.SetActive(false);
//        GradientObj.gameObject.SetActive(false);
//    }
//}
