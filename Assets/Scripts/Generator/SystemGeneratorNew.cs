using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.IO;

public class SystemGeneratorNew : MonoBehaviour
{
    public Star StarPrefab;
    public BlackHole BlackHolePrefab;
    public Planet PlanetPrefab;
    public Asteroid AsteroidPrefab;

    public List<FilePlanet> FilePlanetList = new List<FilePlanet>();
    private List<FilePlanet> hotPlanetList = new List<FilePlanet>();
    private List<FilePlanet> warmPlanetList = new List<FilePlanet>();
    private List<FilePlanet> mildPlanetList = new List<FilePlanet>();
    private List<FilePlanet> coolPlanetList = new List<FilePlanet>();
    private List<FilePlanet> coldPlanetList = new List<FilePlanet>();

    public static SystemGeneratorNew Instance
    {
        get { return instance; }
    }
    private static SystemGeneratorNew instance;

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

        //// Debug
        //FilePlanet filePlanet = new FilePlanet();
        //filePlanet.FileName = "Volcanic.json";
        //filePlanet.Name = "Volcanic";
        //filePlanet.HotWeight = 1;
        //filePlanet.WarmWeight = 0;
        //filePlanet.MildWeight = 0;
        //filePlanet.CoolWeight = 0;
        //filePlanet.ColdWeight = 0;

        //FileHandler.SaveToJSON<FilePlanet>(filePlanet, FileManager.PlanetsFolderPath, filePlanet.FileName);

        //SetPlanetLists();
        //Debug.Log(FilePlanetList[0].Name);
        //Debug.Log("Hot: " + hotPlanetList.Count);
        //Debug.Log("Warm: " + warmPlanetList.Count);
    }

    // SolarSystem
    public void GenerateAllSolarSystems()
    {
        SetPlanetLists();

        System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);

        foreach (SolarSystem system in GalaxyGenerator.SolarSystemList)
        {
            GenerateSolarSystem(system, rand);

            // Add satellites to SatelliteController.SatelliteList
            foreach (Satellite satellite in system.CentralBody.SatelliteList)
            {
                SatelliteController.Instance.CentralBodySatelliteList.Add(satellite);
            }
        }
    }
    public void GenerateSolarSystem(SolarSystem system, System.Random rand)
    {
        // Solar System
        system.transform.rotation = Quaternion.identity; // Reset system rotation (to fix spiral galaxies changing rotation)

        SetRadius(system, rand); // Set System Radius
        SetOrbitDirection(system, rand); // Set Orbit Direction

        system.Name = NameGenerator.GenerateCelestialName(rand); // Generate Name

        // CentralBody
        GenerateCentralBody(system, rand);

        // Satellites
        if (system.CentralBody is Star)
        {
            GenerateSatellites(system, rand);
        }
    }
    private void SetRadius(SolarSystem system, System.Random rand)
    {
        Dictionary<SizeType, int> sizeDictionary = new Dictionary<SizeType, int>();

        sizeDictionary[SizeType.Huge] = 1;
        sizeDictionary[SizeType.Large] = 2;
        sizeDictionary[SizeType.Medium] = 3;
        sizeDictionary[SizeType.Small] = 2;
        sizeDictionary[SizeType.Tiny] = 1;

        system.SizeType = Tools.GetTypeFromWeighted(sizeDictionary, rand);
        system.Radius = SolarSystem.GetRadiusFromSizeType(system.SizeType);
    }
    private void SetOrbitDirection(SolarSystem system, System.Random rand)
    {
        // Set Orbit Direction
        int orbitDirectionSeed = rand.Next(0, 2);

        if (orbitDirectionSeed == 0)
        {
            system.OrbitDirection = Vector3.forward;
        }
        else if (orbitDirectionSeed == 1)
        {
            system.OrbitDirection = Vector3.back;
        }
    }

    // CentralBody
    private void GenerateCentralBody(SolarSystem system, System.Random rand)
    {
        CentralBody centralBody = null;

        // Set type of CentralBody
        Dictionary<string, int> centralBodyDictionary = new Dictionary<string, int>();
        string starString = "Star";
        string blackHoleString = "BlackHole";

        centralBodyDictionary[starString] = 100;
        centralBodyDictionary[blackHoleString] = 1;

        string centralBodyType = Tools.GetTypeFromWeighted(centralBodyDictionary, rand);

        // Generate type of CentralBody
        if (centralBodyType == starString)
        {
            centralBody = GenerateStar(system);
        }
        else if (centralBodyType == blackHoleString)
        {
            centralBody = GenerateBlackHole(system);
        }

        // Generate Temperature
        Dictionary<TemperatureType, int> temperatureDictionary = new Dictionary<TemperatureType, int>();

        temperatureDictionary[TemperatureType.Hot] = 1;
        temperatureDictionary[TemperatureType.Warm] = 2;
        temperatureDictionary[TemperatureType.Mild] = 3;
        temperatureDictionary[TemperatureType.Cool] = 2;
        temperatureDictionary[TemperatureType.Cold] = 1;

        centralBody.TemperatureType = Tools.GetTypeFromWeighted(temperatureDictionary, rand);

        // Set system variables
        system.CentralBody = centralBody;

        // Set star variables
        centralBody.SolarSystem = system;
        centralBody.SizeType = system.SizeType;

        // Set Position
        centralBody.transform.position = Vector3.zero;

        // Set Scale, relative to system radius
        float scale = system.Radius / 20;
        centralBody.transform.localScale = new Vector3(scale, scale, scale);

        // Set Name
        centralBody.Name = system.Name;

        // Set Description
        if (centralBody is Star)
        {
            centralBody.Description = centralBody.TemperatureType.ToString() + " Star";
        }
        else if (centralBody is BlackHole)
        {
            centralBody.Description = "Black Hole";
        }

        //// Set System Material
        //system.circleSprite.GetComponent<Renderer>().sharedMaterial = star.GetComponent<Renderer>().sharedMaterial;
    }
    private Star GenerateStar(SolarSystem system)
    {
        return Instantiate(StarPrefab, system.transform);
    }
    private BlackHole GenerateBlackHole(SolarSystem system)
    {
        return Instantiate(BlackHolePrefab, system.transform);
    }

    // Satellite
    private void GenerateSatellites(SolarSystem system, System.Random rand)
    {
        // Set maximum satellite amount for system
        int maxSatelliteAmount = 0;
        int baseValue = 3;

        switch (system.SizeType)
        {
            case SizeType.Tiny:
                maxSatelliteAmount = baseValue;
                break;
            case SizeType.Small:
                maxSatelliteAmount = baseValue * 2;
                break;
            case SizeType.Medium:
                maxSatelliteAmount = baseValue * 3;
                break;
            case SizeType.Large:
                maxSatelliteAmount = baseValue * 4;
                break;
            case SizeType.Huge:
                maxSatelliteAmount = baseValue * 5;
                break;
        }

        // Set actual satellite amount for system
        int satelliteAmount = rand.Next(0, maxSatelliteAmount + 1);

        // Set remaining variables necessary to generate satellites
        List<int> orbitLayerList = SetOrbitLayerList(maxSatelliteAmount);
        float distanceIncrement = (system.Radius - system.CentralBody.transform.localScale.x) / maxSatelliteAmount;

        // Generate Satellites
        for (int i = 0; i < satelliteAmount; i++)
        {
            // Set orbitLayer for satellite
            int index = rand.Next(0, orbitLayerList.Count);
            int orbitLayer = orbitLayerList[index];
            float standardDistance = system.CentralBody.transform.localScale.x + (distanceIncrement * orbitLayer);
            orbitLayerList.RemoveAt(index);

            // Instantiate Satellite
            List<Satellite> satelliteList = GenerateSatellite(system, rand, orbitLayer, standardDistance);

            foreach (Satellite satellite in satelliteList)
            {
                // Set variables
                satellite.SolarSystem = system;
                satellite.ParentCelestial = system.CentralBody;

                // Add to system's satelliteList
                system.CentralBody.SatelliteList.Add(satellite); // TODO: ONLY PLANETS/MOONS ?

                // Generate Name
                satellite.Name = NameGenerator.GenerateCelestialName(rand);

                //// Set Description
                //string planetType = SetPlanetTypeText(satellite);
                //string surfaceType = SetSurfaceTypeText(satellite);
                //satellite.Description = surfaceType + " " + planetType;
            }
        }
    }
    private List<Satellite> GenerateSatellite(SolarSystem system, System.Random rand, int orbitLayer, float standardDistance)
    {
        List<Satellite> satelliteList = new List<Satellite>();

        // Set type
        Dictionary<string, int> satelliteDictionary = new Dictionary<string, int>();
        string planetString = "Planet";
        string asteroidString = "Asteroid";

        satelliteDictionary[planetString] = 4;
        satelliteDictionary[asteroidString] = 1;

        string satelliteString = Tools.GetTypeFromWeighted(satelliteDictionary, rand);

        // Generate satellite
        if (satelliteString == planetString)
        {
            satelliteList = GeneratePlanet(system, rand, orbitLayer, standardDistance);
        }
        else if (satelliteString == asteroidString)
        {
            satelliteList = GenerateAsteroid(system, rand, orbitLayer, standardDistance);
        }

        return satelliteList;
    }
    private List<Satellite> GeneratePlanet(SolarSystem system, System.Random rand, int orbitLayer, float standardDistance)
    {
        // Set distance & angle
        float distance = standardDistance;
        int angle = rand.Next(0, 360);

        // Set Cartesian Position (Coordinates)
        CartesianCoord cartesianCoord = Tools.ConvertPolarToCartesian(distance, angle);
        Vector3 position = new Vector3(cartesianCoord.x, cartesianCoord.y, 0);

        // Instantiate
        Planet planet = Instantiate(PlanetPrefab, position, Quaternion.identity, system.transform);

        // Set Scale
        int planetScaleSeed = rand.Next(0, 5);
         
        switch (planetScaleSeed)
        {
            case 0:
                planet.SizeType = SizeType.Tiny;
                planet.transform.localScale = new Vector3(2, 2, 1);
                break;
            case 1:
                planet.SizeType = SizeType.Small;
                planet.transform.localScale = new Vector3(2.5f, 2.5f, 1);
                break;
            case 2:
                planet.SizeType = SizeType.Medium;
                planet.transform.localScale = new Vector3(3, 3, 1);
                break;
            case 3:
                planet.SizeType = SizeType.Large;
                planet.transform.localScale = new Vector3(3.5f, 3.5f, 1);
                break;
            case 4:
                planet.SizeType = SizeType.Huge;
                planet.transform.localScale = new Vector3(4, 4, 1);
                break;
        }

        // List
        List<Satellite> satelliteList = new List<Satellite>();
        satelliteList.Add(planet);

        return satelliteList;
    }
    private List<Satellite> GenerateAsteroid(SolarSystem system, System.Random rand, int orbitLayer, float standardDistance)
    {
        // Belt, empty parent object
        GameObject belt = new GameObject();
        belt.name = "Belt";
        belt.transform.position = Vector3.zero;
        belt.transform.SetParent(system.transform);

        // Asteroids, child objects
        List<Satellite> satelliteList = new List<Satellite>();
        List<Vector3> positionList = new List<Vector3>();

        int minAsteroidCount = (int)(standardDistance * 0.2f);
        int maxAsteroidCount = (int)(standardDistance * 0.3f);
        int asteroidCount = rand.Next(minAsteroidCount, maxAsteroidCount);
        Debug.Log("Asteroid Count: " + asteroidCount);

        for (int i = 0; i < asteroidCount; i++)
        {
            // Set position
            Vector3 position;
            float distanceThreshold = 2.5f;
            int loopCountMax = 100;
            int loopCount = 0;

            do
            {
                // Set distance & angle
                float distance = standardDistance;
                int angle = rand.Next(0, 360);

                // Set Cartesian Position (Coordinates)
                CartesianCoord cartesianCoord = Tools.ConvertPolarToCartesian(distance, angle);
                position = new Vector3(cartesianCoord.x, cartesianCoord.y, 0);

                loopCount++;

                if (loopCount > loopCountMax)
                {
                    Debug.Log("Asteroid Generation Collision Error!");
                    break;
                }

            } while (Tools.IsCollision(position, positionList, distanceThreshold));

            // Instantiate
            Asteroid asteroid = Instantiate(AsteroidPrefab, position, Quaternion.identity, belt.transform);

            // Set Scale
            int asteroidScaleSeed = rand.Next(0, 5);

            switch (asteroidScaleSeed)
            {
                case 0:
                    asteroid.SizeType = SizeType.Tiny;
                    asteroid.transform.localScale = new Vector3(0.2f, 0.2f, 1);
                    break;
                case 1:
                    asteroid.SizeType = SizeType.Small;
                    asteroid.transform.localScale = new Vector3(0.25f, 0.25f, 1);
                    break;
                case 2:
                    asteroid.SizeType = SizeType.Medium;
                    asteroid.transform.localScale = new Vector3(0.3f, 0.3f, 1);
                    break;
                case 3:
                    asteroid.SizeType = SizeType.Large;
                    asteroid.transform.localScale = new Vector3(0.35f, 0.35f, 1);
                    break;
                case 4:
                    asteroid.SizeType = SizeType.Huge;
                    asteroid.transform.localScale = new Vector3(0.4f, 0.4f, 1);
                    break;
            }

            satelliteList.Add(asteroid);
            positionList.Add(asteroid.transform.position);
        }

        return satelliteList;
    }

    private void SetPlanetLists()
    {
        // Clear
        FilePlanetList.Clear();
        hotPlanetList.Clear();
        warmPlanetList.Clear();
        mildPlanetList.Clear();
        coolPlanetList.Clear();
        coldPlanetList.Clear();

        // Set
        List<string> fileList = Directory.GetFiles(FileManager.PlanetsFolderPath).ToList();

        foreach (string file in fileList)
        {
            FilePlanet filePlanet = FileHandler.ReadFromJSON<FilePlanet>(FileManager.PlanetsFolderPath, Path.GetFileName(file));

            FilePlanetList.Add(filePlanet);

            if (filePlanet.HotWeight > 0)
            {
                hotPlanetList.Add(filePlanet);
            }
            if (filePlanet.WarmWeight > 0)
            {
                warmPlanetList.Add(filePlanet);
            }
            if (filePlanet.MildWeight > 0)
            {
                mildPlanetList.Add(filePlanet);
            }
            if (filePlanet.CoolWeight > 0)
            {
                coolPlanetList.Add(filePlanet);
            }
            if (filePlanet.ColdWeight > 0)
            {
                coldPlanetList.Add(filePlanet);
            }
        }
    }
    private List<int> SetOrbitLayerList(int maxPlanetAmount)
    {
        List<int> result = new List<int>();

        for (int i = 1; i <= maxPlanetAmount; i++)
        {
            result.Add(i);
        }

        return result;
    }
}
