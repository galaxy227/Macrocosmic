using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates items in systems (Stars, satellites, etc)

public class SystemGenerator : MonoBehaviour
{
    // CentralBody
    public Star RedPrefab;
    public Star OrangePrefab;
    public Star YellowPrefab;
    public Star WhitePrefab;
    public Star BluePrefab;
    public BlackHole BlackHolePrefab;
    // GlowLight for SolarSystem in Galaxy View
    public Material RedStarLight;
    public Material OrangeStarLight;
    public Material YellowStarLight;
    public Material WhiteStarLight;
    public Material BlueStarLight;
    public Material BlackHoleLight;
    // Satellite Prefab
    public Satellite IcePrefab;
    public Satellite OceanicPrefab;
    public Satellite GaiaPrefab;
    public Satellite TemperatePrefab;
    public Satellite DesertPrefab;
    public Satellite VolcanicPrefab;
    public Satellite ToxicPrefab;
    public Satellite BarrenPrefab;
    public Satellite GasGiantPrefab;
    // Satellite SurfaceType Multipliers
    public static float IceMultiplier;
    public static float OceanicMultiplier;
    public static float GaiaMultiplier;
    public static float TemperateMultiplier;
    public static float DesertMultiplier;
    public static float VolcanicMultiplier;
    public static float ToxicMultiplier;
    public static float BarrenMultiplier;
    public static float GasGiantMultiplier;

    public static SystemGenerator Instance;

    private void Awake()
    {
        OnAwake();
    }

    // GENERATION, Solar System
    public void GenerateAllSolarSystems()
    {
        System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);

        foreach (SolarSystem system in GalaxyGenerator.SolarSystemList)
        {
            GenerateSolarSystem(system, rand);

            ViewController.HideSolarSystem(system);
        }

        // Add satellites to GalaxyGenerator.SatelliteList
        foreach (SolarSystem system in GalaxyGenerator.SolarSystemList)
        {
            foreach (Satellite satellite in system.CentralBody.SatelliteList)
            {
                SatelliteController.Instance.CentralBodySatelliteList.Add(satellite);
            }
        }
    }
    public void GenerateSolarSystem(SolarSystem system, System.Random rand)
    {
        InitializeSolarSystem(system, rand);

        // Generate CentralBody
        GenerateCentralBody(system, rand);

        // Generate Satellites
        if (system.CentralBody is Star)
        {
            GenerateSatellites(system, rand);
        }
    } // Generate one individual solar system
    private void InitializeSolarSystem(SolarSystem system, System.Random rand)
    {
        // Reset system rotation (to fix spiral galaxies changing rotation)
        system.transform.rotation = Quaternion.identity;

        // Set System Radius
        SetRadius(system, rand);

        // Set Orbit Direction
        SetOrbitDirection(system, rand);

        // Generate Name
        system.Name = NameGenerator.GenerateCelestialName(rand);
    }
    private void SetRadius(SolarSystem system, System.Random rand)
    {
        int radiusSeed = rand.Next(0, 5);

        // Set RadiusType & Radius value
        if (radiusSeed == 0)
        {
            system.SizeType = SizeType.Tiny;
            system.Radius = SolarSystem.GetRadiusFromSizeType(SizeType.Tiny);
        }
        else if (radiusSeed == 1)
        {
            system.SizeType = SizeType.Small;
            system.Radius = SolarSystem.GetRadiusFromSizeType(SizeType.Small);
        }
        else if (radiusSeed == 2)
        {
            system.SizeType = SizeType.Medium;
            system.Radius = SolarSystem.GetRadiusFromSizeType(SizeType.Medium);
        }
        else if (radiusSeed == 3)
        {
            system.SizeType = SizeType.Large;
            system.Radius = SolarSystem.GetRadiusFromSizeType(SizeType.Large);
        }
        else if (radiusSeed == 4)
        {
            system.SizeType = SizeType.Huge;
            system.Radius = SolarSystem.GetRadiusFromSizeType(SizeType.Huge);
        }
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
        int centralBodyTypeSeed = rand.Next(0, 100);

        if (centralBodyTypeSeed <= 95) // Star
        {
            GenerateStar(system, rand);
        }
        else if (centralBodyTypeSeed <= 99) // Blackhole
        {
            GenerateBlackHole(system);
        }
    }
    private void GenerateStar(SolarSystem system, System.Random rand)
    {
        int starTypeSeed = rand.Next(0, 100);

        Star star = null;

        // Instantiate, Set StarType
        if (starTypeSeed <= 10) // Red
        {
            star = Instantiate(RedPrefab);
            star.starType = Star.StarType.Red;
            Counter.RedCount++;

            system.glowLight.GetComponent<Renderer>().material = RedStarLight;
        }
        else if (starTypeSeed <= 30) // Orange
        {
            star = Instantiate(OrangePrefab);
            star.starType = Star.StarType.Orange;
            Counter.OrangeCount++;

            system.glowLight.GetComponent<Renderer>().material = OrangeStarLight;
        }
        else if (starTypeSeed <= 70) // Yellow
        {
            star = Instantiate(YellowPrefab);
            star.starType = Star.StarType.Yellow;
            Counter.YellowCount++;

            system.glowLight.GetComponent<Renderer>().material = YellowStarLight;
        }
        else if (starTypeSeed <= 90) // White
        {
            star = Instantiate(WhitePrefab);
            star.starType = Star.StarType.White;
            Counter.WhiteCount++;

            system.glowLight.GetComponent<Renderer>().material = WhiteStarLight;
        }
        else if (starTypeSeed <= 99) // Blue
        {
            star = Instantiate(BluePrefab);
            star.starType = Star.StarType.Blue;
            Counter.BlueCount++;

            system.glowLight.GetComponent<Renderer>().material = BlueStarLight;
        }

        // Initialize
        InitializeCentralBody(system, star);
    }
    private void GenerateBlackHole(SolarSystem system)
    {
        // Instantiate 
        BlackHole blackHole = Instantiate(BlackHolePrefab);
        Counter.BlackHoleCount++;

        system.glowLight.GetComponent<Renderer>().material = BlackHoleLight;

        // Initialize
        InitializeCentralBody(system, blackHole);
    }
    private void InitializeCentralBody(SolarSystem system, CentralBody centralBody)
    {
        //Set System Material
        system.circleSprite.GetComponent<Renderer>().sharedMaterial = centralBody.GetComponent<Renderer>().sharedMaterial;

        // Set Parent
        centralBody.transform.SetParent(system.transform, true);
        system.CentralBody = centralBody;

        // Set Position
        centralBody.transform.position = Vector3.zero;

        // Scale CentralBody relative to system radius
        float scale = system.Radius / 15;
        centralBody.transform.localScale = new Vector3(scale, scale, scale);

        // Set System
        centralBody.SolarSystem = system;

        // Set Name
        centralBody.Name = system.Name;

        // Set Description
        if (centralBody is Star)
        {
            centralBody.Description = SetStarTypeText((Star)centralBody);
        }
        else if (centralBody is BlackHole)
        {
            centralBody.Description = "Black Hole";
        }
    }
    // CentralBody Helper
    private string SetStarTypeText(Star star)
    {
        string result = "";

        if (star.starType == Star.StarType.Blue)
        {
            result = "Hot";
        }
        else if (star.starType == Star.StarType.White)
        {
            result = "Warm";
        }
        else if (star.starType == Star.StarType.Yellow)
        {
            result = "Mild";
        }
        else if (star.starType == Star.StarType.Orange)
        {
            result = "Cool";
        }
        else if (star.starType == Star.StarType.Red)
        {
            result = "Cold";
        }

        return result + " Star";
    }

    // Satellite
    private void GenerateSatellites(SolarSystem system, System.Random rand)
    {
        // Generate Planets
        GeneratePlanets(system, rand);
    }
    private void GeneratePlanets(SolarSystem system, System.Random rand)
    {
        Satellite satellite = null;
        int maxPlanetAmount = 0;
        int planetAmount = 0;
        int orbitLayer = 0; // each orbit is a "layer" (keep 1 planet on 1 orbital path)
        List<int> orbitLayerList = new List<int>(); // keep track of which orbitLayers have been taken by a planet, so planets dont have same orbitLayer
        float distanceIncrement = 0; // distance between orbitLayers for Polar Position
        float centralBodyScale = system.CentralBody.transform.localScale.x;

        // Set maxPlanetAmount 
        int baseValue = 3; // each maxPlanetAmount must be multiple of baseValue

        if (system.SizeType == SizeType.Tiny)
        {
            maxPlanetAmount = baseValue;
        }
        else if (system.SizeType == SizeType.Small)
        {
            maxPlanetAmount = baseValue * 2;
        }
        else if (system.SizeType == SizeType.Medium)
        {
            maxPlanetAmount = baseValue * 3;
        }
        else if (system.SizeType == SizeType.Large)
        {
            maxPlanetAmount = baseValue * 4;
        }
        else if (system.SizeType == SizeType.Huge)
        {
            maxPlanetAmount = baseValue * 5;
        }

        // Set planet amount with Random.Next, set distanceIncrement, set orbitLayerList
        planetAmount = rand.Next(0, maxPlanetAmount + 1); // add 1 because the System.Random max is non-inclusive
        distanceIncrement = (system.Radius - centralBodyScale) / maxPlanetAmount;
        orbitLayerList = SetOrbitLayerList(maxPlanetAmount);

        // Generate Planets
        for (int i = 0; i < planetAmount; i++)
        {
            // Set orbitLayer for planet
            int index = rand.Next(0, orbitLayerList.Count);
            orbitLayer = orbitLayerList[index];
            orbitLayerList.RemoveAt(index);

            // Set distance & angle
            float distanceOfFirstOrbitLayer = centralBodyScale + distanceIncrement;
            float distanceFromCentralBody = distanceOfFirstOrbitLayer + (distanceIncrement * (orbitLayer - 1));
            int angle = rand.Next(0, 360);

            // Set Cartesian Position (Coordinates)
            CartesianCoord cartesianCoord = Tools.ConvertPolarToCartesian(distanceFromCentralBody, angle);
            Vector3 position = new Vector3(cartesianCoord.x, cartesianCoord.y, 0);

            // Instantiate, Set PlanetType
            int groupValue = maxPlanetAmount / baseValue; // ie. maxPlanetAmount = 15 / 3 ... groupValue = 5 (includes orbitLayer: 1, 2, 3, 4, 5), groupValue * 2 = 10 (includes orbitLayer: 6, 7, 8, 9, 10), groupValue * 3 = 15 (includes orbitLayer: 11, 12, 13, 14, 15)

            if (system.CentralBody is Star)
            {
                Star star = (Star)system.CentralBody;

                if (star.starType == Star.StarType.Blue) // HIGHEST TEMPERATURE
                {
                    if (orbitLayer == 1) // Hottest Orbit of Hottest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Volcanic.Weight = 1;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue) // Hottest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Volcanic.Weight = 6;
                        planetProbability.Toxic.Weight = 2;
                        planetProbability.Barren.Weight = 1;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue * 2)
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Toxic.Weight = 2;
                        planetProbability.Desert.Weight = 3;
                        planetProbability.Barren.Weight = 3;
                        planetProbability.GasGiant.Weight = 1;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue * 3) // Coldest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Oceanic.Weight = 1;
                        planetProbability.Temperate.Weight = 1;
                        planetProbability.Barren.Weight = 3;
                        planetProbability.GasGiant.Weight = 4;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                }
                else if (star.starType == Star.StarType.White) // HIGH TEMPERATURE
                {
                    if (orbitLayer == 1) // Hottest Orbit of Hottest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Volcanic.Weight = 1;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue) // Hottest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Volcanic.Weight = 4;
                        planetProbability.Toxic.Weight = 2;
                        planetProbability.Barren.Weight = 1;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue * 2)
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Toxic.Weight = 1;
                        planetProbability.Desert.Weight = 2;
                        planetProbability.Barren.Weight = 2;
                        planetProbability.GasGiant.Weight = 1;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue * 3) // Coldest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Oceanic.Weight = 6;
                        planetProbability.Temperate.Weight = 6;
                        planetProbability.Gaia.Weight = 1;
                        planetProbability.Barren.Weight = 9;
                        planetProbability.GasGiant.Weight = 12;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                }
                else if (star.starType == Star.StarType.Yellow) // MEDIUM TEMPERATURE
                {
                    if (orbitLayer == 1) // Hottest Orbit of Hottest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Volcanic.Weight = 3;
                        planetProbability.Toxic.Weight = 2;
                        planetProbability.Barren.Weight = 1;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue) // Hottest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Toxic.Weight = 1;
                        planetProbability.Desert.Weight = 2;
                        planetProbability.Barren.Weight = 2;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue * 2)
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Oceanic.Weight = 4;
                        planetProbability.Temperate.Weight = 4;
                        planetProbability.Gaia.Weight = 1;
                        planetProbability.Barren.Weight = 8;
                        planetProbability.GasGiant.Weight = 6;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue * 3) // Coldest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Ice.Weight = 1;
                        planetProbability.Barren.Weight = 1;
                        planetProbability.GasGiant.Weight = 2;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                }
                else if (star.starType == Star.StarType.Orange) // LOW TEMPERATURE
                {
                    if (orbitLayer == 1) // Hottest Orbit of Hottest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Temperate.Weight = 1;
                        planetProbability.Desert.Weight = 2;
                        planetProbability.Barren.Weight = 2;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue) // Hottest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Oceanic.Weight = 6;
                        planetProbability.Temperate.Weight = 6;
                        planetProbability.Gaia.Weight = 1;
                        planetProbability.Barren.Weight = 18;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue * 2)
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Ice.Weight = 1;
                        planetProbability.Barren.Weight = 1;
                        planetProbability.GasGiant.Weight = 2;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue * 3) // Coldest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Ice.Weight = 2;
                        planetProbability.Barren.Weight = 1;
                        planetProbability.GasGiant.Weight = 2;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                }
                else if (star.starType == Star.StarType.Red) // LOWEST TEMPERATURE
                {
                    if (orbitLayer == 1) // Hottest Orbit of Hottest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Toxic.Weight = 1;
                        planetProbability.Desert.Weight = 1;
                        planetProbability.Barren.Weight = 2;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue) // Hottest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Oceanic.Weight = 1;
                        planetProbability.Temperate.Weight = 1;
                        planetProbability.Barren.Weight = 6;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue * 2)
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Ice.Weight = 1;
                        planetProbability.Barren.Weight = 1;
                        planetProbability.GasGiant.Weight = 2;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                    else if (orbitLayer <= groupValue * 3) // Coldest Layer
                    {
                        PlanetProbability planetProbability = new PlanetProbability();

                        planetProbability.Ice.Weight = 3;
                        planetProbability.Barren.Weight = 1;
                        planetProbability.GasGiant.Weight = 2;

                        satellite = SpawnPlanetType(planetProbability.GetRandomSurfaceType(rand), position, rand);
                    }
                }
            }

            // Set Parent, Add to system's satelliteList
            satellite.transform.SetParent(system.transform, true);
            system.CentralBody.SatelliteList.Add(satellite);

            // Set System
            satellite.SolarSystem = system;

            // Set ParentCelestial
            satellite.ParentCelestial = system.CentralBody;

            // Set Name
            satellite.Name = NameGenerator.GenerateCelestialName(rand);

            // Set Description
            string planetType = SetPlanetTypeText(satellite);
            string surfaceType = SetSurfaceTypeText(satellite);
            satellite.Description = surfaceType + " " + planetType;
        }
    }
    private Satellite SpawnPlanetType(SurfaceType surfaceType, Vector3 position, System.Random rand)
    {
        Satellite satellite = null;
        
        // Instantiate
        if (surfaceType == SurfaceType.Ice) // Ice
        {
            satellite = Instantiate(IcePrefab, position, IcePrefab.transform.rotation);
            satellite.SurfaceType = SurfaceType.Ice;
            Counter.IceCount++;
        }
        else if (surfaceType == SurfaceType.Oceanic) // Oceanic
        {
            satellite = Instantiate(OceanicPrefab, position, OceanicPrefab.transform.rotation);
            satellite.SurfaceType = SurfaceType.Oceanic;
            Counter.OceanicCount++;
        }
        else if (surfaceType == SurfaceType.Gaia) // Gaia
        {
            satellite = Instantiate(GaiaPrefab, position, GaiaPrefab.transform.rotation);
            satellite.SurfaceType = SurfaceType.Gaia;
            Counter.GaiaCount++;
        }
        else if (surfaceType == SurfaceType.Temperate) // Temperate
        {
            satellite = Instantiate(TemperatePrefab, position, TemperatePrefab.transform.rotation);
            satellite.SurfaceType = SurfaceType.Temperate;
            Counter.TemperateCount++;
        }
        else if (surfaceType == SurfaceType.Desert) // Desert
        {
            satellite = Instantiate(DesertPrefab, position, DesertPrefab.transform.rotation);
            satellite.SurfaceType = SurfaceType.Desert;
            Counter.DesertCount++;
        }
        else if (surfaceType == SurfaceType.Volcanic) // Volcanic
        {
            satellite = Instantiate(VolcanicPrefab, position, VolcanicPrefab.transform.rotation);
            satellite.SurfaceType = SurfaceType.Volcanic;
            Counter.VolcanicCount++;
        }
        else if (surfaceType == SurfaceType.Toxic) // Toxic
        {
            satellite = Instantiate(ToxicPrefab, position, ToxicPrefab.transform.rotation);
            satellite.SurfaceType = SurfaceType.Toxic;
            Counter.ToxicCount++;
        }
        else if (surfaceType == SurfaceType.Barren) // Barren
        {
            satellite = Instantiate(BarrenPrefab, position, BarrenPrefab.transform.rotation);
            satellite.SurfaceType = SurfaceType.Barren;
            Counter.BarrenCount++;
        }
        else if (surfaceType == SurfaceType.GasGiant) // GasGiant
        {
            satellite = Instantiate(GasGiantPrefab, position, GasGiantPrefab.transform.rotation);
            satellite.SurfaceType = SurfaceType.GasGiant;
            Counter.GasGiantCount++;
        }

        // PlanetType
        satellite.SatelliteType = SatelliteType.Planet;

        // Scale
        int planetScaleSeed = rand.Next(0, 5);

        if (satellite.SurfaceType == SurfaceType.GasGiant) // Gas Giant
        {
            if (planetScaleSeed == 0) // Tiny
            {
                satellite.transform.localScale = new Vector3(4, 4, 4);
            }
            else if (planetScaleSeed == 1) // Small
            {
                satellite.transform.localScale = new Vector3(5, 5, 5);
            }
            else if (planetScaleSeed == 2) // Medium
            {
                satellite.transform.localScale = new Vector3(6, 6, 6);
            }
            else if (planetScaleSeed == 3) // Large
            {
                satellite.transform.localScale = new Vector3(7, 7, 7);
            }
            else if (planetScaleSeed == 4) // Huge
            {
                satellite.transform.localScale = new Vector3(8, 8, 8);
            }
        }
        else // Other SurfaceTypes
        {
            if (planetScaleSeed == 0) // Tiny
            {
                satellite.transform.localScale = new Vector3(2, 2, 2);
            }
            else if (planetScaleSeed == 1) // Small
            {
                satellite.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            }
            else if (planetScaleSeed == 2) // Medium
            {
                satellite.transform.localScale = new Vector3(3, 3, 3);
            }
            else if (planetScaleSeed == 3) // Large
            {
                satellite.transform.localScale = new Vector3(3.5f, 3.5f, 3.5f);
            }
            else if (planetScaleSeed == 4) // Huge
            {
                satellite.transform.localScale = new Vector3(4, 4, 4);
            }
        }

        // SizeType
        if (planetScaleSeed == 0) // Tiny
        {
            satellite.SizeType = SizeType.Tiny;
        }
        else if (planetScaleSeed == 1) // Small
        {
            satellite.SizeType = SizeType.Small;
        }
        else if (planetScaleSeed == 2) // Medium
        {
            satellite.SizeType = SizeType.Medium;
        }
        else if (planetScaleSeed == 3) // Large
        {
            satellite.SizeType = SizeType.Large;
        }
        else if (planetScaleSeed == 4) // Huge
        {
            satellite.SizeType = SizeType.Huge;
        }

        return satellite;
    }
    // Satellite Helper
    private List<int> SetOrbitLayerList(int maxPlanetAmount)
    {
        List<int> result = new List<int>();

        for (int i = 1; i <= maxPlanetAmount; i++)
        {
            result.Add(i);
        }

        return result;
    }
    private string SetPlanetTypeText(Satellite satellite)
    {
        int value = (int)satellite.SatelliteType;
        string result = System.Enum.GetName(typeof(SatelliteType), value);

        if (satellite.SurfaceType == SurfaceType.GasGiant)
        {
            result = "";
        }

        return result;
    }
    private string SetSurfaceTypeText(Satellite satellite)
    {
        int value = (int)satellite.SurfaceType;
        string result = System.Enum.GetName(typeof(SurfaceType), value);

        if (result == "GasGiant")
        {
            result = "Gas Giant";
        }

        return result;
    }

    // Utility
    private void OnAwake()
    {
        Instance = this;

        SetPlanetMultipliers();
    }
    private void SetPlanetMultipliers()
    {
        IceMultiplier = 1f;
        OceanicMultiplier = 1f;
        GaiaMultiplier = 1f;
        TemperateMultiplier = 1f;
        DesertMultiplier = 1f;
        VolcanicMultiplier = 1f;
        ToxicMultiplier = 1f;
        BarrenMultiplier = 1f;
        GasGiantMultiplier = 1f;
    }

    public class PlanetProbability
    {
        public PlanetWeight Ice = new PlanetWeight(SurfaceType.Ice);
        public PlanetWeight Oceanic = new PlanetWeight(SurfaceType.Oceanic);
        public PlanetWeight Gaia = new PlanetWeight(SurfaceType.Gaia);
        public PlanetWeight Temperate = new PlanetWeight(SurfaceType.Temperate);
        public PlanetWeight Desert = new PlanetWeight(SurfaceType.Desert);
        public PlanetWeight Volcanic = new PlanetWeight(SurfaceType.Volcanic);
        public PlanetWeight Toxic = new PlanetWeight(SurfaceType.Toxic);
        public PlanetWeight Barren = new PlanetWeight(SurfaceType.Barren);
        public PlanetWeight GasGiant = new PlanetWeight(SurfaceType.GasGiant);

        public float SumOfWeights;

        private List<PlanetWeight> PlanetWeightList = new List<PlanetWeight>();

        public PlanetProbability()
        {
            PlanetWeightList.Add(Ice);
            PlanetWeightList.Add(Oceanic);
            PlanetWeightList.Add(Gaia);
            PlanetWeightList.Add(Temperate);
            PlanetWeightList.Add(Desert);
            PlanetWeightList.Add(Volcanic);
            PlanetWeightList.Add(Toxic);
            PlanetWeightList.Add(Barren);
            PlanetWeightList.Add(GasGiant);
        }

        public SurfaceType GetRandomSurfaceType(System.Random rand)
        {
            SurfaceType surfaceType = SurfaceType.Barren;

            SumOfWeights = 0;

            // Set SumOfWeights and PlanetWeight Values
            for (int i = 0; i < PlanetWeightList.Count; i++)
            {
                float initialSum = SumOfWeights;
                SumOfWeights += PlanetWeightList[i].Weight;

                PlanetWeightList[i].Min = initialSum;
                PlanetWeightList[i].Max = SumOfWeights;
            }

            // Calculate seed
            float seed = 0;

            do
            {
                seed = (float)rand.NextDouble();
            }
            while (seed <= 0);

            float seedValue = Mathf.Lerp(0, SumOfWeights, seed);

            // Set surfaceType
            for (int i = 0; i < PlanetWeightList.Count; i++)
            {
                if (PlanetWeightList[i].Weight != 0) // Do not iterate over PlanetWeights without an assigned weight value
                {
                    if (seedValue > PlanetWeightList[i].Min && seedValue <= PlanetWeightList[i].Max)
                    {
                        surfaceType = PlanetWeightList[i].SurfaceType;
                    }
                }
            }

            return surfaceType;
        }

        public class PlanetWeight
        {
            public SurfaceType SurfaceType;
            public float Weight
            {
                get
                {
                    return weight;
                }
                set
                {
                    weight = value * GetMultiplier(SurfaceType);
                }
            }
            private float weight;
            public float Min;
            public float Max;

            public PlanetWeight(SurfaceType surfaceType)
            {
                SurfaceType = surfaceType;
            }

            public float GetMultiplier(SurfaceType surfaceType)
            {
                float multiplier = 0;

                switch (surfaceType)
                {
                    case SurfaceType.Ice:
                        multiplier = IceMultiplier;
                        break;
                    case SurfaceType.Oceanic:
                        multiplier = OceanicMultiplier;
                        break;
                    case SurfaceType.Gaia:
                        multiplier = GaiaMultiplier;
                        break;
                    case SurfaceType.Temperate:
                        multiplier = TemperateMultiplier;
                        break;
                    case SurfaceType.Desert:
                        multiplier = DesertMultiplier;
                        break;
                    case SurfaceType.Volcanic:
                        multiplier = VolcanicMultiplier;
                        break;
                    case SurfaceType.Toxic:
                        multiplier = ToxicMultiplier;
                        break;
                    case SurfaceType.Barren:
                        multiplier = BarrenMultiplier;
                        break;
                    case SurfaceType.GasGiant:
                        multiplier = GasGiantMultiplier;
                        break;
                }

                return multiplier;
            }
        }
    }
}
