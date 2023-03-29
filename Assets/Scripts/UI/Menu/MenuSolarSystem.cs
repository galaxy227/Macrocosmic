using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSolarSystem : MonoBehaviour
{
    private SolarSystem solarSystem;

    void Start()
    {
        OnStart();
    }
    void Update()
    {
        if (solarSystem != null)
        {
            UpdateMainMenuSatelliteOrbits();
        }
    }

    // MainMenuSolarSystem
    private void UpdateMainMenuSatelliteOrbits()
    {
        foreach (Satellite satellite in solarSystem.CentralBody.SatelliteList)
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
            solarSystem = Instantiate(GalaxyGenerator.Instance.systemPrefab, Vector3.zero, GalaxyGenerator.Instance.systemPrefab.transform.rotation);
            solarSystem.transform.SetParent(transform, true);

            // Generate SolarSystem
            SystemGenerator.Instance.GenerateSolarSystem(solarSystem, rand);

            if (solarSystem.CentralBody.SatelliteList.Count < minPlanets)
            {
                Destroy(solarSystem.gameObject);
            }

        } while (solarSystem.CentralBody.SatelliteList.Count < minPlanets);

        // Set View
        InputManager.SelectedSolarSystem = solarSystem;
        ViewController.SetSystemView(InputManager.SelectedSolarSystem);

        // Set Camera
        PlayerCamera.Instance.SetCameraPosition(new Vector3(PlayerCamera.Instance.transform.position.z * 0.35f, PlayerCamera.Instance.transform.position.z * 0.025f, PlayerCamera.Instance.transform.position.z));
    }

    private void OnStart()
    {
        GenerateMainMenuSolarSystem();

        GameController.ChangeGameState.AddListener(OnChangeGameState);
    }
    private void OnChangeGameState()
    {
        Debug.Log("ChangeGameState");

        if (GameController.Instance.GameState != GameState.MainMenu)
        {
            // Set View
            InputManager.SelectedSolarSystem = solarSystem;
            ViewController.SetGalaxyView(solarSystem);

            Destroy(solarSystem.gameObject);
        }
    }
}
