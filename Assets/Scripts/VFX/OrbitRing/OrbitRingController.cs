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

    // Orbit Rings
    private void UpdateOrbitRingWidth()
    {
        if (ViewController.ViewType == ViewType.System)
        {
            float minWidth = 0.000001f;
            float maxWidth = 0.25f * (InputManager.SelectedSolarSystem.radius / SolarSystem.GetRadiusFromSizeType(SizeType.Tiny));

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
    private void InstantiateOrbitRings()
    {
        orbitRingList.Clear();

        for (int i = 0; i < 15; i++) // 15 is maximum possible planets
        {
            OrbitRing orbitRing = Instantiate(orbitRingPrefab, new Vector3(0, 0, -0.01f), orbitRingPrefab.transform.rotation);
            orbitRing.transform.SetParent(transform);
            orbitRingList.Add(orbitRing);

            ObjectHelper.HideObject(orbitRing.gameObject);
        }
    }
    private void RevealOrbitRings()
    {
        for (int i = 0; i < InputManager.SelectedSolarSystem.satelliteList.Count; i++)
        {
            ObjectHelper.RevealObject(orbitRingList[i].gameObject);

            orbitRingList[i].xScale = InputManager.SelectedSolarSystem.satelliteList[i].GetDistance();
            orbitRingList[i].yScale = InputManager.SelectedSolarSystem.satelliteList[i].GetDistance();
            orbitRingList[i].CalculateEllipse();
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
        InstantiateOrbitRings();

        ViewController.ChangeView.AddListener(OnChangeView);
    }
}
