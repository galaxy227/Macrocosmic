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
            sizeType = SizeType.Tiny;
        }
        else if (solarSystem.radiusType == SolarSystem.RadiusType.Small)
        {
            sizeType = SizeType.Small;
        }
        else if (solarSystem.radiusType == SolarSystem.RadiusType.Medium)
        {
            sizeType = SizeType.Medium;
        }
        else if (solarSystem.radiusType == SolarSystem.RadiusType.Large)
        {
            sizeType = SizeType.Large;
        }
        else if (solarSystem.radiusType == SolarSystem.RadiusType.Huge)
        {
            sizeType = SizeType.Huge;
        }
    }
}
