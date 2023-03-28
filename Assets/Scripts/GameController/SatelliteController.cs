using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls orbits for all satellites

public class SatelliteController : MonoBehaviour
{
    public List<Satellite> CentralBodySatelliteList = new List<Satellite>(); // Satellites orbiting CentralBody

    private const float orbitSpeed = 1f;

    public static SatelliteController Instance
    {
        get {  return instance; }
    }
    private static SatelliteController instance;

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
    void Update()
    {
        OrbitCentralBodySatellites();
    }

    public static void OrbitCentralBodySatellites()
    {
        if (TimeController.Instance.SpeedType != SpeedType.Paused)
        {
            foreach (Satellite satellite in Instance.CentralBodySatelliteList)
            {
                OrbitSatellite(satellite);
            }
        }
    }
    public static void OrbitSatellite(Satellite satellite)
    {
        PolarCoord polarCoord = Tools.ConvertCartesianToPolar(satellite.transform.position.x, satellite.transform.position.y);
        float angleOffset = (1 / satellite.SpawnDistance) * orbitSpeed * TimeController.Instance.customDeltaTime;

        if (satellite.SolarSystem.OrbitDirection == Vector3.back)
        {
            angleOffset *= -1f;
        }

        float targetAngle = polarCoord.angle + angleOffset;
        CartesianCoord cartesianCoord = Tools.ConvertPolarToCartesian(satellite.SpawnDistance, targetAngle);

        if (!float.IsNaN(cartesianCoord.x) && !float.IsNaN(cartesianCoord.y))
        {
            satellite.transform.position = new Vector3(cartesianCoord.x, cartesianCoord.y, satellite.transform.position.z) + satellite.ParentCelestial.transform.position;
        }
    }
}

//// Unity rotation with distance bug
//if (TimeController.speedType != TimeController.SpeedType.Paused)
//{
//    float angle = (1 / spawnDistance) * orbitSpeed * TimeController.customDeltaTime;

//    // Rotate
//    transform.RotateAround(solarSystem.centralBody.transform.position, solarSystem.orbitDirection, angle);
//    transform.rotation = Quaternion.identity;
//}

//// Unity rotation with distance fix (poor performance)
//if (TimeController.speedType != TimeController.SpeedType.Paused)
//{
//    float angle = (1 / spawnDistance) * orbitSpeed * TimeController.customDeltaTime;

//    // Rotate
//    transform.RotateAround(solarSystem.centralBody.transform.position, solarSystem.orbitDirection, angle);
//    transform.rotation = Quaternion.identity;

//    // Fix distance (bug where planet's distance from centralBody would change overtime)
//    float currentAngle = Mathf.Atan2(transform.position.y, transform.position.x);
//    CartesianCoord cartesianCoord = Tools.ConvertPolarToCartesian(spawnDistance, currentAngle);
//    transform.position = new Vector3(cartesianCoord.x, cartesianCoord.y, transform.position.z);
//}
