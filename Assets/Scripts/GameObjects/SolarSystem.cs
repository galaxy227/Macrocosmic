using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SolarSystem : Unit
{
    // VFX
    public GameObject glowLight;
    public GameObject circleSprite;

    public CentralBody centralBody;
    public List<Satellite> satelliteList = new List<Satellite>();

    public SizeType SizeType;
    public Vector3 orbitDirection;
    public int radius; // physical radius of system, determined by size

    private void Awake()
    {
        OnAwake();
    }
    void Start()
    {
        OnStart();
    }

    // Utility
    protected override void OnAwake()
    {
        base.OnAwake();
    }
    protected override void OnStart()
    {
        base.OnStart();
    }
    public static int GetRadiusFromSizeType(SizeType sizeType)
    {
        switch (sizeType)
        {
            case SizeType.Tiny:
                return 100;
            case SizeType.Small:
                return 200;
            case SizeType.Medium:
                return 300;
            case SizeType.Large:
                return 400;
            case SizeType.Huge:
                return 500;
            default: 
                return 0;
        }
    }
}
