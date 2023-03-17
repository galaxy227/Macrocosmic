using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemGeneratorNew : MonoBehaviour
{
    public static SystemGeneratorNew Instance;
    private static SystemGeneratorNew instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void GenerateAllSolarSystems()
    {

    }
    private void GenerateSolarSystem()
    {

    }
}
