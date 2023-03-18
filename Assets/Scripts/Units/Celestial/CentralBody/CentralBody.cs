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
        if (solarSystem.SizeType == SizeType.Tiny)
        {
            SizeType = SizeType.Tiny;
        }
        else if (solarSystem.SizeType == SizeType.Small)
        {
            SizeType = SizeType.Small;
        }
        else if (solarSystem.SizeType == SizeType.Medium)
        {
            SizeType = SizeType.Medium;
        }
        else if (solarSystem.SizeType == SizeType.Large)
        {
            SizeType = SizeType.Large;
        }
        else if (solarSystem.SizeType == SizeType.Huge)
        {
            SizeType = SizeType.Huge;
        }
    }
}
