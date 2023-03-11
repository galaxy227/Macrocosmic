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

    public enum RadiusType
    {
        Tiny = 150,
        Small = 300,
        Medium = 450,
        Large = 600,
        Huge = 750
    }

    public RadiusType radiusType;
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
}
