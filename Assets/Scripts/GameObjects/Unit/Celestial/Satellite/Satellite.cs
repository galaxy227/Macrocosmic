using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SatelliteType
{
    Planet,
    Moon
}

public enum SurfaceType
{
    Ice,
    Oceanic,
    Gaia,
    Temperate,
    Desert,
    Volcanic,
    Toxic,
    Barren,
    GasGiant,
}

// Celestial orbiting around another Celestial (Planet around Star, Moon around Planet, etc)

public class Satellite : Celestial
{
    public SatelliteType SatelliteType;
    public SurfaceType SurfaceType;

    // Position
    private float spawnDistance; // distance from orbited celestial when instantiated (only accurate at spawn, as exact float value varies once satellite begins orbitting)
    public float SpawnDistance
    {
        get { return spawnDistance; }
    }

    // Utility
    protected override void OnStart()
    {
        base.OnStart();

        spawnDistance = GetDistance();
    }
    public float GetDistance()
    {
        float distance = (solarSystem.centralBody.transform.position - transform.position).magnitude;

        return distance;
    }
}