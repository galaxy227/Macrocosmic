using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Any CentralBody of a SolarSystem or Satellite orbiting a CentralBody (Star, planet, moon, etc)

public class Celestial : Unit
{
    public SolarSystem solarSystem;
    public Size size;

    private void Start()
    {
        OnStart();
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    public struct Size // use Struct because Enum cannot be inherited 
    {
        public enum SizeType
        {
            Tiny,
            Small,
            Medium,
            Large,
            Huge
        }

        public SizeType sizeType;
    }
}
