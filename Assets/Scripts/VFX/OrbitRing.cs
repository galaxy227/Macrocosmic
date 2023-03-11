using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class OrbitRing : MonoBehaviour
{
    public LineRenderer lr;

    public int segments;
    public float xScale;
    public float yScale;

    void Awake()
    {
        lr = GetComponent<LineRenderer>();
        CalculateEllipse();
    }

    public void CalculateEllipse()
    {
        Vector3[] points = new Vector3[segments + 1];
        for (int i = 0; i < segments; i++)
        {
            float angle = ((float)i / (float)segments) * 360 * Mathf.Deg2Rad;
            float x = Mathf.Sin(angle) * xScale;
            float y = Mathf.Cos(angle) * yScale;
            points[i] = new Vector3(x, y, 0f);
        }
        points[segments] = points[0];

        if (lr != null)
        {
            lr.positionCount = segments + 1;
            lr.SetPositions(points);
        }
    }

    private void OnValidate()
    {
        if (Application.isPlaying)
        {
            CalculateEllipse();
        }
    }
}
