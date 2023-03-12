using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// CoreLight.cs controls the object rendering light from the center of the Galaxy

public class CoreLight : MonoBehaviour
{
    private Material coreLightMaterial;

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

        SetCLSize();
        SetCLLight();
        SetCLColor();
        SetCLSimpleNoise();
    }
    private void SetCLSize()
    {
        float scale = 200 * ((int)GalaxyGenerator.Instance.sizeType + 1);

        gameObject.transform.localScale = new Vector3(scale, scale, 1);
    }
    private void SetCLLight()
    {
        if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ring)
        {
            coreLightMaterial.SetFloat("_Light", 0.7f);
        }
        else
        {
            coreLightMaterial.SetFloat("_Light", 0.65f);
        }
    }
    private void SetCLColor()
    {
        coreLightMaterial.SetColor("_Color", ColorGenerator.Mixed);
    }
    private void SetCLSimpleNoise()
    {
        // Seed (Simple Noise Offset)
        System.Random rand = new System.Random(GalaxyGenerator.Instance.Seed);
        int seed = rand.Next(0, 10000);
        coreLightMaterial.SetFloat("_Seed", seed);

        // Noise Divider
        if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Spiral)
        {
            coreLightMaterial.SetFloat("_NoiseDivider", 15f);
        }
        else if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ellipitical)
        {
            coreLightMaterial.SetFloat("_NoiseDivider", 15f);
        }
        else if (GalaxyGenerator.Instance.shapeType == GalaxyGenerator.ShapeType.Ring)
        {
            coreLightMaterial.SetFloat("_NoiseDivider", 20f);
        }
    }

    // Utility
    private void OnStart()
    {
        coreLightMaterial = GetComponent<Renderer>().sharedMaterial;

        gameObject.SetActive(false);

        GalaxyGenerator.AfterGenerate.AddListener(OnGenerate);
        ViewController.ChangeView.AddListener(OnChangeView);
    }
}
