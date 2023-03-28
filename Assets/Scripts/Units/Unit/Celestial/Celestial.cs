using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TemperatureType
{
    Hot,
    Warm,
    Mild,
    Cool,
    Cold
}

// Any CentralBody of a SolarSystem or Satellite orbiting a CentralBody (Star, planet, moon, etc)
public class Celestial : Unit
{
    public SizeType SizeType;
    public TemperatureType TemperatureType;
    public SolarSystem SolarSystem;
    public List<Satellite> SatelliteList = new List<Satellite>();

    private void Start()
    {
        OnStart();
    }

    protected override void OnStart()
    {
        base.OnStart();
    }
}
