using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// SatelliteType
public class FilePlanet
{
    public string FileName;

    public string Name;

    // Temperature
    public int HotWeight
    {
        get { return Mathf.Clamp(HotWeight, minWeight, maxWeight); }
        set { HotWeight = value; }
    }
    public int WarmWeight
    {
        get { return Mathf.Clamp(WarmWeight, minWeight, maxWeight); }
        set { WarmWeight = value; }
    }
    public int MildWeight
    {
        get { return Mathf.Clamp(MildWeight, minWeight, maxWeight); }
        set { MildWeight = value; }
    }
    public int CoolWeight
    {
        get { return Mathf.Clamp(CoolWeight, minWeight, maxWeight); }
        set { CoolWeight = value; }
    }
    public int ColdWeight
    {
        get { return Mathf.Clamp(ColdWeight, minWeight, maxWeight); }
        set { ColdWeight = value; }
    }
    private const int maxWeight = 100;
    private const int minWeight = 0;
}
