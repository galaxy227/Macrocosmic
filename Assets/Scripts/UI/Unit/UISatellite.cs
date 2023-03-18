using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UISatellite : UIUnit
{
    [Header("Summary")]
    public TextMeshProUGUI Size;
    [Header("Debug")]
    public TextMeshProUGUI Distance;
    public TextMeshProUGUI SpawnDistance;

    protected override void OnEnable()
    {
        base.OnEnable();

        if (unit != null)
        {
            UpdateCanvas();
        }
    }

    protected override void UpdateCanvas()
    {
        base.UpdateCanvas();

        UpdateSummary();
        UpdateDebug();
    }

    // Summary
    private void UpdateSummary()
    {
        if (unit is Satellite satellite)
        {
            Size.text = SetSizeTypeText(satellite);
        }
    }
    private string SetSizeTypeText(Satellite satellite)
    {
        int value = (int)satellite.SizeType;
        string result = System.Enum.GetName(typeof(SizeType), value);

        return result;
    }

    // Debug
    private void UpdateDebug()
    {
        if (unit is Satellite satellite)
        {
            if (Console.DebugMode)
            {
                // Active
                if (!Distance.gameObject.activeSelf)
                {
                    Distance.gameObject.SetActive(true);
                }
                if (!SpawnDistance.gameObject.activeSelf)
                {
                    SpawnDistance.gameObject.SetActive(true);
                }

                // Text
                Distance.text = "Distance: " + SetDistanceText(satellite);
                SpawnDistance.text = "Spawn Distance: " + System.Math.Round(satellite.SpawnDistance, 2).ToString();
            }
            else
            {
                // Active
                if (Distance.gameObject.activeSelf)
                {
                    Distance.gameObject.SetActive(false);
                }
                if (SpawnDistance.gameObject.activeSelf)
                {
                    SpawnDistance.gameObject.SetActive(false);
                }
            }
        }
    }
    private string SetDistanceText(Satellite satellite)
    {
        float distance = Vector3.Distance(satellite.transform.position, satellite.solarSystem.centralBody.transform.position);
        float value = (float)System.Math.Round(distance, 2);
        string result = value.ToString();

        return result;
    }
}

//public class UISatellite : MonoBehaviour
//{
//    // Title
//    public TextMeshProUGUI Name;
//    public TextMeshProUGUI Description;
//    // Summary
//    public TextMeshProUGUI Size;
//    [Header("Mask")]
//    public RectMask2D Mask;
//    public DelayType DelayType;
//    private float secondsToFade;
//    [Header("Debug")]
//    public TextMeshProUGUI Distance;
//    public TextMeshProUGUI SpawnDistance;

//    private void OnEnable()
//    {
//        StartCoroutine(HandleMask());
//        UpdateSatellitePanel();
//    }

//    private void UpdateSatellitePanel()
//    {
//        UpdateText();
//    }

//    // Text
//    private void UpdateText()
//    {
//        if (InputManager.SelectedUnit is Satellite)
//        {
//            Satellite satellite = (Satellite)InputManager.SelectedUnit;

//            // Name
//            Name.text = satellite.Name;

//            // Description
//            string planetType = SetPlanetTypeText(satellite);
//            string surfaceType = SetSurfaceTypeText(satellite);
//            Description.text = surfaceType + " " + planetType;

//            // Size
//            Size.text = SetSizeTypeText(satellite);

//            // Distance
//            if (Console.DebugMode)
//            {
//                // Active
//                if (!Distance.gameObject.activeSelf)
//                {
//                    Distance.gameObject.SetActive(true);
//                }
//                if (!SpawnDistance.gameObject.activeSelf)
//                {
//                    SpawnDistance.gameObject.SetActive(true);
//                }

//                // Text
//                Distance.text = "Distance: " + SetDistanceText(satellite);
//                SpawnDistance.text = "Spawn Distance: " + System.Math.Round(satellite.SpawnDistance, 2).ToString();
//            }
//            else
//            {
//                // Active
//                if (Distance.gameObject.activeSelf)
//                {
//                    Distance.gameObject.SetActive(false);
//                }
//                if (SpawnDistance.gameObject.activeSelf)
//                {
//                    SpawnDistance.gameObject.SetActive(false);
//                }
//            }
//        }
//    }
//    // Text Helpers
//    private string SetPlanetTypeText(Satellite satellite)
//    {
//        int value = (int)satellite.SatelliteType;
//        string result = System.Enum.GetName(typeof(SatelliteType), value);

//        if (satellite.SurfaceType == SurfaceType.GasGiant)
//        {
//            result = "";
//        }

//        return result;
//    }
//    private string SetSurfaceTypeText(Satellite satellite)
//    {
//        int value = (int)satellite.SurfaceType;
//        string result = System.Enum.GetName(typeof(SurfaceType), value);

//        if (result == "GasGiant")
//        {
//            result = "Gas Giant";
//        }

//        return result;
//    }
//    private string SetSizeTypeText(Satellite satellite)
//    {
//        int value = (int)satellite.size.sizeType;
//        string result = System.Enum.GetName(typeof(Celestial.Size.SizeType), value);

//        return result;
//    }
//    private string SetDistanceText(Satellite satellite)
//    {
//        float distance = Vector3.Distance(satellite.transform.position, satellite.solarSystem.centralBody.transform.position);
//        float value = (float)System.Math.Round(distance, 2);
//        string result = value.ToString();

//        return result;
//    }
//    // Mask
//    private IEnumerator HandleMask()
//    {
//        // Declare variables
//        secondsToFade = DelayHelper.GetDelayTime(DelayType);
//        float initialWidth = Mask.gameObject.GetComponent<RectTransform>().sizeDelta.x;
//        float minimumPadding = 0f;
//        Mask.padding = new Vector4(initialWidth, minimumPadding);
//        bool isIncompleteFade = false;

//        do
//        {
//            isIncompleteFade = false;

//            float increment = (initialWidth / secondsToFade) * Time.unscaledDeltaTime;
//            float newPadding = Mathf.Clamp(Mask.padding.x - increment, minimumPadding, initialWidth);

//            Mask.padding = new Vector4(newPadding, minimumPadding);

//            if (newPadding > minimumPadding)
//            {
//                isIncompleteFade = true;
//            }

//            yield return null;

//        } while (isIncompleteFade);

//        StopCoroutine(HandleMask());
//    }
//}
