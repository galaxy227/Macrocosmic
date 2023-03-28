using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

public enum ViewType
{
    Galaxy,
    System
}

public enum SubViewType
{
    Low,
    High
}


public class ViewController : MonoBehaviour
{
    public static ViewType ViewType;
    public static SubViewType SubViewType;
    public static UnityEvent ChangeView;
    public static UnityEvent ChangeSubView;

    // SubView variables
    public static float lowBarrierGalaxy;
    public static float lowBarrierSystem;

    private void Awake()
    {
        OnAwake();
    }
    private void Start()
    {
        OnStart();
    }
    private void Update()
    {
        UpdateSubViewType();
    }

    // Set SubView
    private static void UpdateSubViewType()
    {
        if (ViewType == ViewType.Galaxy)
        {
            SetSubViewType(lowBarrierGalaxy);
        }
        else if (ViewType == ViewType.System)
        {
            SetSubViewType(lowBarrierSystem);
        }
    }
    private static void SetSubViewType(float lowBarrier)
    {
        if (PlayerCamera.Instance.transform.position.z <= lowBarrier)
        {
            if (SubViewType != SubViewType.Low)
            {
                SubViewType = SubViewType.Low;

                ChangeSubView.Invoke();
            }
        }
        else
        {
            if (SubViewType != SubViewType.High)
            {
                SubViewType = SubViewType.High;

                ChangeSubView.Invoke();
            }
        }
    }

    // Set Views
    public static void SetGalaxyView(SolarSystem system)
    {
        ViewType = ViewType.Galaxy;

        if (system != null)
        {
            HideSolarSystem(system);
        }

        RevealGalaxy();
        SetCameraBounds(GalaxyGenerator.Instance.Radius);

        ChangeView.Invoke();
    }
    public static void SetSystemView(SolarSystem system)
    {
        if (system != null)
        {
            ViewType = ViewType.System;

            HideGalaxy();

            RevealSolarSystem(system);
            SetCameraBounds(system.Radius);

            ChangeView.Invoke();
        }
    }

    // Galaxy
    private static void RevealGalaxy()
    {
        foreach (SolarSystem system in GalaxyGenerator.SolarSystemList)
        {
            ObjectHelper.RevealObject(system.gameObject);
            ObjectHelper.RevealObject(system.glowLight);
            ObjectHelper.RevealObject(system.circleSprite);
        }
    }
    private static void HideGalaxy()
    {
        foreach (SolarSystem system in GalaxyGenerator.SolarSystemList)
        {
            ObjectHelper.HideObject(system.gameObject);
            ObjectHelper.HideObject(system.glowLight);
            ObjectHelper.HideObject(system.circleSprite);
        }
    }

    // Solar System
    private static void RevealSolarSystem(SolarSystem system)
    {
        if (system != null)
        {
            // CentralBody
            ObjectHelper.RevealObjectAndChildren(system.CentralBody.gameObject);

            // Satellites
            foreach (Satellite satellite in system.CentralBody.SatelliteList)
            {
                ObjectHelper.RevealObject(satellite.gameObject);
            }
        }
    }
    public static void HideSolarSystem(SolarSystem system)
    {
        if (system != null)
        {
            // CentralBody
            ObjectHelper.HideObjectAndChildren(system.CentralBody.gameObject);

            // Satellites
            foreach (Satellite satellite in system.CentralBody.SatelliteList)
            {
                ObjectHelper.HideObject(satellite.gameObject);
            }
        }
    }

    // Camera
    public static void SetCameraBounds(int radius)
    {
        PlayerCamera cam = PlayerCamera.Instance;

        cam.MaxZoom = radius * 2.5f;
        cam.MaxPan = radius * 2.5f;

        if (ViewType == ViewType.Galaxy)
        {
            if (InputManager.SelectedSolarSystem == null) // before player selects any system, ie. after galaxy generation
            {
                cam.SetCameraPosition(new Vector3(0, 0, cam.MaxZoom));
            }
            else
            {
                cam.SetCameraPosition(new Vector3(InputManager.SelectedSolarSystem.transform.position.x, InputManager.SelectedSolarSystem.transform.position.y, 35));
            }

            cam.ZoomBarrier = 10f;
        }
        else if (ViewType == ViewType.System)
        {
            cam.SetCameraPosition(new Vector3(0, 0, cam.MaxZoom));

            cam.ZoomBarrier = 0.1f;
        }
    } 

    // Utility
    private void OnAwake()
    {
        ViewType = ViewType.Galaxy;

        if (ChangeView == null)
        {
            ChangeView = new UnityEvent();
        }

        if (ChangeSubView == null)
        {
            ChangeSubView = new UnityEvent();
        }
    }
    private void OnStart()
    {
        GalaxyGenerator.AfterGenerate.AddListener(OnGenerate);
    }
    private void OnGenerate()
    {
        SetSubViewBarriers();

        SetGalaxyView(null);

        ChangeSubView.Invoke();
    }
    private void SetSubViewBarriers()
    {
        lowBarrierGalaxy = GalaxyGenerator.Instance.Radius * 0.5f;
        lowBarrierSystem = SolarSystem.GetRadiusFromSizeType(SizeType.Tiny);
    }
}

public static class ObjectHelper
{
    // Object Helpers
    public static void RevealObject(GameObject obj)
    {
        if (obj.TryGetComponent(out Renderer renderer))
        {
            renderer.enabled = true;
        }

        if (obj.TryGetComponent(out Collider2D collider))
        {
            collider.enabled = true;
        }
    }
    public static void RevealObjectAndChildren(GameObject obj)
    {
        List<GameObject> objectList = new List<GameObject>();
        objectList = Tools.GetAllChildren(obj, true);

        foreach (GameObject objectInList in objectList)
        {
            if (objectInList.TryGetComponent(out Renderer renderer))
            {
                renderer.enabled = true;
            }

            if (objectInList.TryGetComponent(out Collider2D collider))
            {
                collider.enabled = true;
            }
        }
    }
    public static void HideObject(GameObject obj)
    {
        if (obj.TryGetComponent(out Renderer renderer))
        {
            renderer.enabled = false;
        }

        if (obj.TryGetComponent(out Collider2D collider))
        {
            collider.enabled = false;
        }
    }
    public static void HideObjectAndChildren(GameObject obj)
    {
        List<GameObject> objectList = new List<GameObject>();
        objectList = Tools.GetAllChildren(obj, true);

        foreach (GameObject objectInList in objectList)
        {
            if (objectInList.TryGetComponent(out Renderer renderer))
            {
                renderer.enabled = false;
            }

            if (objectInList.TryGetComponent(out Collider2D collider))
            {
                collider.enabled = false;
            }
        }
    }
}
