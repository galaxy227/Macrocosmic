using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// The central body of a Solar System (Star, BlackHole)

public class CentralBody : Celestial
{
    protected override void OnStart()
    {
        base.OnStart();

        SetSizeType();
    }
    private void SetSizeType()
    {
        if (SolarSystem.SizeType == SizeType.Tiny)
        {
            SizeType = SizeType.Tiny;
        }
        else if (SolarSystem.SizeType == SizeType.Small)
        {
            SizeType = SizeType.Small;
        }
        else if (SolarSystem.SizeType == SizeType.Medium)
        {
            SizeType = SizeType.Medium;
        }
        else if (SolarSystem.SizeType == SizeType.Large)
        {
            SizeType = SizeType.Large;
        }
        else if (SolarSystem.SizeType == SizeType.Huge)
        {
            SizeType = SizeType.Huge;
        }
    }
}
