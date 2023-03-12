using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSolarSystem : MonoBehaviour
{
    public static SolarSystem SolarSystemInstance
    {
        get { return solarSystemInstance; }
    }
    private static SolarSystem solarSystemInstance;

    void Start()
    {
        OnStart();
    }
    void Update()
    {
        if (solarSystemInstance != null)
        {
            UpdateMainMenuSatelliteOrbits();
        }
    }

    // MainMenuSolarSystem
    private void UpdateMainMenuSatelliteOrbits()
    {
        foreach (Satellite satellite in solarSystemInstance.satelliteList)
        {
            SatelliteController.OrbitSatellite(satellite);
        }
    }
    private void GenerateMainMenuSolarSystem()
    {
        System.Random rand = new System.Random();
        int minPlanets = 4;

        do
        {
            // Instantiate SolarSystem
            solarSystemInstance = Instantiate(GalaxyGenerator.Instance.systemPrefab, Vector3.zero, GalaxyGenerator.Instance.systemPrefab.transform.rotation);
            solarSystemInstance.transform.SetParent(transform, true);

            // Generate SolarSystem
            SystemGenerator.Instance.GenerateSolarSystem(solarSystemInstance, rand);

            if (solarSystemInstance.satelliteList.Count < minPlanets)
            {
                Destroy(solarSystemInstance.gameObject);
            }

        } while (solarSystemInstance.satelliteList.Count < minPlanets);

        // Set View
        InputManager.SelectedSolarSystem = solarSystemInstance;
        ViewController.SetSystemView(solarSystemInstance);

        // Set Camera
        float z = PlayerCamera.Instance.transform.position.z;
        PlayerCamera.Instance.transform.position = new Vector3(PlayerCamera.Instance.transform.position.x + (z * 0.35f), PlayerCamera.Instance.transform.position.y + (z * 0.025f), z);
    }

    private void OnStart()
    {
        GenerateMainMenuSolarSystem();

        GameController.ChangeGameState.AddListener(OnChangeGameState);
    }
    private void OnChangeGameState()
    {
        if (GameController.Instance.GameState != GameState.MainMenu)
        {
            // Set View
            InputManager.SelectedSolarSystem = solarSystemInstance;
            ViewController.SetGalaxyView(solarSystemInstance);

            Destroy(solarSystemInstance.gameObject);
        }
    }
}
