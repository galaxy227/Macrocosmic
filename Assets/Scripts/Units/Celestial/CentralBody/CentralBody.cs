using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The central body of a Solar System, of which all satellites orbit around
// TYPES: STAR, BLACKHOLE

public class CentralBody : Celestial
{
    protected override void OnStart()
    {
        base.OnStart();

        SetSizeType();
    }
    private void SetSizeType()
    {
        if (solarSystem.radiusType == SolarSystem.RadiusType.Tiny)
        {
            size.sizeType = Size.SizeType.Tiny;
        }
        else if (solarSystem.radiusType == SolarSystem.RadiusType.Small)
        {
            size.sizeType = Size.SizeType.Small;
        }
        else if (solarSystem.radiusType == SolarSystem.RadiusType.Medium)
        {
            size.sizeType = Size.SizeType.Medium;
        }
        else if (solarSystem.radiusType == SolarSystem.RadiusType.Large)
        {
            size.sizeType = Size.SizeType.Large;
        }
        else if (solarSystem.radiusType == SolarSystem.RadiusType.Huge)
        {
            size.sizeType = Size.SizeType.Huge;
        }
    }
}
