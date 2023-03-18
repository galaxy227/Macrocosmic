using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string Name;
    public string Description;

    private void Awake()
    {
        OnAwake();
    }
    void Start()
    {
        OnStart();
    }

    // Utility
    protected virtual void OnAwake()
    {

    }
    protected virtual void OnStart()
    {

    }
}
