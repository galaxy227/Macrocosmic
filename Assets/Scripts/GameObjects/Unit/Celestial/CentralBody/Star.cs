using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : CentralBody
{
    public enum StarType
    {
        Blue,
        White,
        Yellow,
        Orange,
        Red
    }

    public StarType starType;

    protected override void OnStart()
    {
        base.OnStart();
    }
}
