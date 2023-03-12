using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

//int starTypeAmount = System.Enum.GetNames(typeof(Star.StarType)).Length;

public static class Tools
{
    // UI
    public static bool IsMouseOverUI()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }

    // Generation
    public static double GetMirroredGaussianDistributionFloat(System.Random rand)
    {
        // CODE FROM ONLINE FOR STANDARD DEVIATION (BELL CURVE) "Box-Muller"
        float standardDeviation = 1; // default standard deviation is 1, larger number is more dispersed
        float mean = 0;
        double randNormal = 0; // float to return

        do
        {
            double u1 = 1.0 - rand.NextDouble(); // uniform (0, 1) random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = System.Math.Sqrt(-2.0 * System.Math.Log(u1)) * System.Math.Sin(2.0 * System.Math.PI * u2); // random normal (0, 1)
            randNormal = mean + standardDeviation * randStdNormal; // random normal (mean, stdDev^2)

        } while (randNormal < -3 || randNormal > 3);

        // Lerp value between 0 and 1
        randNormal = Mathf.InverseLerp(-3, 3, (float)randNormal); // Standard Deviation (Bell Curve) is between -3 and 3

        // "Mirror" values below 0.5f (0 becomes 1f, 0.25f becomes 0.75f, 0.49f becomes 0.51f)
        randNormal = Mathf.Abs((float)randNormal - 1);

        // InverseLerp values between 0.5f and 1f (0.5f becomes 0, 0.75f becomes 0.5f, 1f becomes 1f)
        randNormal = Mathf.InverseLerp(0.5f, 1f, (float)randNormal);

        return randNormal;
    } // 0 is most common, 1 is most rare

    // Coordinates
    public static CartesianCoord ConvertPolarToCartesian(float radius, float angle)
    {
        CartesianCoord coord = new CartesianCoord();

        coord.x = Mathf.Cos(angle) * radius;
        coord.y = Mathf.Sin(angle) * radius;

        return coord;
    }
    public static PolarCoord ConvertCartesianToPolar(float x, float y)
    {
        PolarCoord coord = new PolarCoord();

        coord.radius = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2));
        coord.angle = Mathf.Atan2(y, x);

        return coord;
    }

    // Objects
    public static List<GameObject> GetAllChildren(GameObject parent, bool includeParentInList)
    {
        List<GameObject> childList = new List<GameObject>();

        if (includeParentInList)
        {
            childList.Add(parent);
        }

        foreach (Transform child in parent.transform)
        {
            childList.Add(child.gameObject);

            if (child.transform.childCount > 0)
            {
                GetAllChildren(child.gameObject, false);
            }
        }

        return childList;
    }

    // Particles
    public static void ClearParticleSystem(ParticleSystem particleSystem)
    {
        if (particleSystem != null)
        {
            particleSystem.Stop();
            particleSystem.Clear();
        }
    }
}

public struct CartesianCoord
{
    public float x;
    public float y;
}
public struct PolarCoord
{
    public float radius;
    public float angle;
}
