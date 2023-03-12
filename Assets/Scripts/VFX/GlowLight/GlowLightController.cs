using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls Offset of all GlowLight Shader materials

public class GlowLightController : MonoBehaviour
{
    List<Material> materialList = new List<Material>();

    public Material coreLight;
    public Material redStar;
    public Material orangeStar;
    public Material yellowStar;
    public Material whiteStar;
    public Material blueStar;
    public Material blackHole;

    private float increment = 0.0001f;
    private float startOffset = 100;

    void Start()
    {
        OnStart();
    }

    private void Update()
    {
        UpdateOffset();
    }

    private void UpdateOffset()
    {
        if (TimeController.Instance.SpeedType != SpeedType.Paused)
        {
            float offset = materialList[0].GetFloat("_Offset");
            float newOffset = offset + (increment * (int)TimeController.Instance.SpeedType);

            foreach (Material mat in materialList)
            {
                mat.SetFloat("_Offset", newOffset);
            }
        }
    }

    private void OnGenerate()
    {
        foreach (Material mat in materialList)
        {
            mat.SetFloat("_Offset", startOffset);
        }
    }

    // Utility
    private void OnStart()
    {
        SetMaterialList();

        foreach (Material mat in materialList)
        {
            mat.SetFloat("_Offset", startOffset);
        }

        GalaxyGenerator.AfterGenerate.AddListener(OnGenerate);
    }
    private void SetMaterialList()
    {
        materialList.Add(coreLight);
        materialList.Add(redStar);
        materialList.Add(orangeStar);
        materialList.Add(yellowStar);
        materialList.Add(whiteStar);
        materialList.Add(blueStar);
        materialList.Add(blackHole);
    }
}
