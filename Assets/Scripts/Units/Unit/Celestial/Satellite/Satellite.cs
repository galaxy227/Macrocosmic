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

// Satellite orbits around ParentCelestial
public class Satellite : Celestial
{
    public SatelliteType SatelliteType;
    public SurfaceType SurfaceType;
    public Celestial ParentCelestial; // Celestial which Satellite orbits around

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

        spawnDistance = GetDistanceFromParentCelestial();
    }
    public float GetDistanceFromParentCelestial()
    {
        float distance = (ParentCelestial.transform.position - transform.position).magnitude;

        return distance;
    }
}