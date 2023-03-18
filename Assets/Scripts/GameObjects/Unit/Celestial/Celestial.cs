using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Any CentralBody of a SolarSystem or Satellite orbiting a CentralBody (Star, planet, moon, etc)

public class Celestial : Unit
{
    public SolarSystem solarSystem;
    public SizeType SizeType;

    private void Start()
    {
        OnStart();
    }

    protected override void OnStart()
    {
        base.OnStart();
    }
}
