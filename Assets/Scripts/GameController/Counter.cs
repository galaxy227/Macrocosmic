using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// For keeping track of counts

public static class Counter
{
    // SolarSystem
    public static int SystemCount;
    // CentralBody
    public static int RedCount;
    public static int OrangeCount;
    public static int YellowCount;
    public static int WhiteCount;
    public static int BlueCount;
    public static int BlackHoleCount;
    // Planet
    public static int IceCount; 
    public static int OceanicCount;
    public static int GaiaCount;
    public static int TemperateCount;
    public static int DesertCount;
    public static int VolcanicCount;
    public static int ToxicCount;
    public static int BarrenCount;
    public static int GasGiantCount;
    // Totals
    public static int PlanetCount;

    public static void ResetAllCounts()
    {
        // SolarSystem
        SystemCount = 0;

        // CentralBody
        RedCount = 0;
        OrangeCount = 0;
        YellowCount = 0;
        WhiteCount = 0;
        BlueCount = 0;
        BlackHoleCount = 0;

        // Planet
        IceCount = 0;
        OceanicCount = 0;
        GaiaCount = 0;
        TemperateCount = 0;
        DesertCount = 0;
        VolcanicCount = 0;
        ToxicCount = 0;
        BarrenCount = 0;
        GasGiantCount = 0;

        // Totals 
        PlanetCount = 0;
    }

    public static void SetTotalCounts()
    {
        PlanetCount = IceCount + OceanicCount + GaiaCount + TemperateCount + DesertCount + VolcanicCount + ToxicCount + BarrenCount + GasGiantCount;
    }
}
