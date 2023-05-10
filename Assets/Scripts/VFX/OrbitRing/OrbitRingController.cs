using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Object pool for orbit rings in Solar System view

public class OrbitRingController : MonoBehaviour
{
    public OrbitRing orbitRingPrefab;
    public static List<OrbitRing> orbitRingList = new List<OrbitRing>();

    void Start()
    {
        OnStart();
    }
    void Update()
    {
        UpdateOrbitRingWidth();
    }

    // Update
    private void UpdateOrbitRingWidth()
    {
        if (ViewController.ViewType == ViewType.System)
        {
            float minWidth = 0.000001f;
            float maxWidth = 0.25f * (InputManager.SelectedSolarSystem.Radius / SolarSystem.GetRadiusFromSizeType(SizeType.Tiny));

            float thresholdToDisappear = 25f; // Orbit rings should disappear at this position (Z)
            float thresholdPercentage = Mathf.InverseLerp(PlayerCamera.Instance.MinZoom, PlayerCamera.Instance.MaxZoom, thresholdToDisappear);
            float newZoomPercentage = Mathf.InverseLerp(thresholdPercentage, 1f, PlayerCamera.Instance.ZoomPercentage);

            float width = Mathf.Lerp(minWidth, maxWidth, newZoomPercentage);

            foreach (OrbitRing orbitRing in orbitRingList)
            {
                orbitRing.lr.startWidth = width;
                orbitRing.lr.endWidth = width;
            }
        }
    }

    // Event
    private void OnChangeView()
    {
        if (ViewController.ViewType == ViewType.Galaxy)
        {
            HideOrbitRings();
        }
        else if (ViewController.ViewType == ViewType.System)
        {
            RevealOrbitRings();
        }
    }

    private OrbitRing InstantiateOrbitRing()
    {
        OrbitRing orbitRing = Instantiate(orbitRingPrefab, new Vector3(0, 0, -0.01f), orbitRingPrefab.transform.rotation);
        orbitRing.transform.SetParent(transform);
        orbitRingList.Add(orbitRing);

        ObjectHelper.HideObject(orbitRing.gameObject);

        return orbitRing;
    }
    private void RevealOrbitRings()
    {
        for (int i = 0; i < InputManager.SelectedSolarSystem.CentralBody.SatelliteList.Count; i++)
        {
            // Non-Asteroid Satellites
            if (!(InputManager.SelectedSolarSystem.CentralBody.SatelliteList[i] is Asteroid))
            {
                // Set orbitRing
                OrbitRing orbitRing = null;

                if (i < orbitRingList.Count)
                {
                    orbitRing = orbitRingList[i];
                }
                else
                {
                    orbitRing = InstantiateOrbitRing();
                }

                // Reveal orbitRing
                ObjectHelper.RevealObject(orbitRing.gameObject);

                orbitRing.xScale = InputManager.SelectedSolarSystem.CentralBody.SatelliteList[i].GetDistanceFromParentCelestial();
                orbitRing.yScale = InputManager.SelectedSolarSystem.CentralBody.SatelliteList[i].GetDistanceFromParentCelestial();
                orbitRing.CalculateEllipse();
            }
        }
    }
    private void HideOrbitRings()
    {
        foreach (OrbitRing orbitRing in orbitRingList)
        {
            ObjectHelper.HideObject(orbitRing.gameObject);
        }
    }

    // Utility
    private void OnStart()
    {
        orbitRingList.Clear();

        ViewController.ChangeView.AddListener(OnChangeView);
    }
}


//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//// Object pool for orbit rings in Solar System view

//public class OrbitRingController : MonoBehaviour
//{
//    public OrbitRing orbitRingPrefab;
//    public static List<OrbitRing> orbitRingList = new List<OrbitRing>();

//    void Start()
//    {
//        OnStart();
//    }
//    void Update()
//    {
//        UpdateOrbitRingWidth();
//    }

//    private void OnChangeView()
//    {
//        if (ViewController.ViewType == ViewType.Galaxy)
//        {
//            HideOrbitRings();
//        }
//        else if (ViewController.ViewType == ViewType.System)
//        {
//            RevealOrbitRings();
//        }
//    }

//    // Orbit Rings
//    private void UpdateOrbitRingWidth()
//    {
//        if (ViewController.ViewType == ViewType.System)
//        {
//            float minWidth = 0.000001f;
//            float maxWidth = 0.25f * (InputManager.SelectedSolarSystem.Radius / SolarSystem.GetRadiusFromSizeType(SizeType.Tiny));

//            float thresholdToDisappear = 25f; // Orbit rings should disappear at this position (Z)
//            float thresholdPercentage = Mathf.InverseLerp(PlayerCamera.Instance.MinZoom, PlayerCamera.Instance.MaxZoom, thresholdToDisappear);
//            float newZoomPercentage = Mathf.InverseLerp(thresholdPercentage, 1f, PlayerCamera.Instance.ZoomPercentage);

//            float width = Mathf.Lerp(minWidth, maxWidth, newZoomPercentage);

//            foreach (OrbitRing orbitRing in orbitRingList)
//            {
//                orbitRing.lr.startWidth = width;
//                orbitRing.lr.endWidth = width;
//            }
//        }
//    }
//    private void InstantiateOrbitRings()
//    {
//        orbitRingList.Clear();

//        for (int i = 0; i < 15; i++) // 15 is maximum possible planets
//        {
//            OrbitRing orbitRing = Instantiate(orbitRingPrefab, new Vector3(0, 0, -0.01f), orbitRingPrefab.transform.rotation);
//            orbitRing.transform.SetParent(transform);
//            orbitRingList.Add(orbitRing);

//            ObjectHelper.HideObject(orbitRing.gameObject);
//        }
//    }
//    private void RevealOrbitRings()
//    {
//        for (int i = 0; i < InputManager.SelectedSolarSystem.CentralBody.SatelliteList.Count; i++)
//        {
//            ObjectHelper.RevealObject(orbitRingList[i].gameObject);

//            orbitRingList[i].xScale = InputManager.SelectedSolarSystem.CentralBody.SatelliteList[i].GetDistanceFromParentCelestial();
//            orbitRingList[i].yScale = InputManager.SelectedSolarSystem.CentralBody.SatelliteList[i].GetDistanceFromParentCelestial();
//            orbitRingList[i].CalculateEllipse();
//        }
//    }
//    private void HideOrbitRings()
//    {
//        foreach (OrbitRing orbitRing in orbitRingList)
//        {
//            ObjectHelper.HideObject(orbitRing.gameObject);
//        }
//    }

//    // Utility
//    private void OnStart()
//    {
//        InstantiateOrbitRings();

//        ViewController.ChangeView.AddListener(OnChangeView);
//    }
//}
