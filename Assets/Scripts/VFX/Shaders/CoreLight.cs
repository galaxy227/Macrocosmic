using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// CoreLight.cs controls the object rendering light from the center of the Galaxy

public class CoreLight : MonoBehaviour
{
    private Material mat;

    void Start()
    {
        OnStart();
    }

    // Change View
    private void OnChangeView()
    {
        if (ViewController.ViewType == ViewType.Galaxy)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Generate
    private void OnGenerate()
    {
        gameObject.SetActive(true);

        SetSize();
        SetLight();
        SetColor();
        SetSimpleNoise();
    }
    private void SetSize()
    {
        float scale = 200 * ((int)GalaxyGenerator.Instance.sizeType + 1);

        gameObject.transform.localScale = new Vector3(scale, scale, 1);
    }
    private void SetLight()
    {
        if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ring)
        {
            mat.SetFloat("_Light", 0.7f);
        }
        else
        {
            mat.SetFloat("_Light", 0.65f);
        }
    }
    private void SetColor()
    {
        mat.SetColor("_Color", ColorGenerator.Mixed);
    }
    private void SetSimpleNoise()
    {
        // Seed (Simple Noise Offset)
        System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);
        int seed = rand.Next(0, 10000);
        mat.SetFloat("_Seed", seed);

        // Noise Divider
        if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Spiral)
        {
            mat.SetFloat("_NoiseDivider", 15f);
        }
        else if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ellipitical)
        {
            mat.SetFloat("_NoiseDivider", 15f);
        }
        else if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ring)
        {
            mat.SetFloat("_NoiseDivider", 20f);
        }
    }

    // Utility
    private void OnStart()
    {
        mat = GetComponent<Renderer>().sharedMaterial;

        gameObject.SetActive(false);

        GalaxyGenerator.AfterGenerate.AddListener(OnGenerate);
        ViewController.ChangeView.AddListener(OnChangeView);
    }
}
