using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;
using System.Linq;

public class MenuNew : MonoBehaviour
{
    // Galaxy
    public TMP_InputField inputFieldSeed;
    public TMP_Dropdown dropDownSizeType;
    public TMP_Dropdown dropDownShapeType;
    public TMP_Dropdown dropDownNumArms;
    public GameObject BlockerArms;

    // Satellite SurfaceType Multipliers
    private float baseMultiplier = 0.25f;
    public Slider sliderIce;
    public TextMeshProUGUI textIceMultiplier;
    public Slider sliderOceanic;
    public TextMeshProUGUI textOceanicMultiplier;
    public Slider sliderGaia;
    public TextMeshProUGUI textGaiaMultiplier;
    public Slider sliderTemperate;
    public TextMeshProUGUI textTemperateMultiplier;
    public Slider sliderDesert;
    public TextMeshProUGUI textDesertMultiplier;
    public Slider sliderVolcanic;
    public TextMeshProUGUI textVolcanicMultiplier;
    public Slider sliderToxic;
    public TextMeshProUGUI textToxicMultiplier;
    public Slider sliderBarren;
    public TextMeshProUGUI textBarrenMultiplier;
    public Slider sliderGasGiant;
    public TextMeshProUGUI textGasGiantMultiplier;

    private System.Random rand;

    void Start()
    {
        OnStart();
    }

    // Dropdown
    public void HandleDropDownSizeType(int value)
    {
        if (value == 0)
        {
            GalaxyGenerator.Instance.sizeType = GalaxyGenerator.SizeType.Tiny;
        }
        else if (value == 1)
        {
            GalaxyGenerator.Instance.sizeType = GalaxyGenerator.SizeType.Small;
        }
        else if (value == 2)
        {
            GalaxyGenerator.Instance.sizeType = GalaxyGenerator.SizeType.Medium;
        }
        else if (value == 3)
        {
            GalaxyGenerator.Instance.sizeType = GalaxyGenerator.SizeType.Large;
        }
        else if (value == 4)
        {
            GalaxyGenerator.Instance.sizeType = GalaxyGenerator.SizeType.Huge;
        }

        UpdateDropDownNumArmsOnChangeSizeType(value);
    }
    public void HandleDropDownShapeType(int value)
    {
        if (value == 0)
        {
            GalaxyGenerator.Instance.shapeType = GalaxyGenerator.ShapeType.Spiral;
            BlockerArms.SetActive(false);
        }
        else if (value == 1)
        {
            GalaxyGenerator.Instance.shapeType = GalaxyGenerator.ShapeType.Ellipitical;
            BlockerArms.SetActive(true);
        }
        else if (value == 2)
        {
            GalaxyGenerator.Instance.shapeType = GalaxyGenerator.ShapeType.Ring;
            BlockerArms.SetActive(true);
        }
    }
    public void HandleDropDownNumArms(int value)
    {
        if (value == 0)
        {
            GalaxyGenerator.Instance.NumArms = 2;
        }
        else if (value == 1)
        {
            GalaxyGenerator.Instance.NumArms = 3;
        }
        else if (value == 2)
        {
            GalaxyGenerator.Instance.NumArms = 4;
        }
        else if (value == 3)
        {
            GalaxyGenerator.Instance.NumArms = 5;
        }
        else if (value == 4)
        {
            GalaxyGenerator.Instance.NumArms = 6;
        }
    }

    // Input Field
    public void HandleInputFieldSeed(string input)
    {
        if (string.IsNullOrEmpty(input))
        {
            GalaxyGenerator.Instance.inputSeed = 0;
        }
        else
        {
            if (Int32.TryParse(input, out int value))
            {
                if (value < 0)
                {
                    value = Mathf.Abs(value);
                }
            }
            else
            {
                value = 0;
            }

            GalaxyGenerator.Instance.inputSeed = value;
        }
    }

    // Slider
    public void HandleIceMultiplier()
    {
        SystemGenerator.IceMultiplier = baseMultiplier * sliderIce.value;
        textIceMultiplier.text = SystemGenerator.IceMultiplier.ToString() + "x";
    }
    public void HandleOceanicMultiplier()
    {
        SystemGenerator.OceanicMultiplier = baseMultiplier * sliderOceanic.value;
        textOceanicMultiplier.text = SystemGenerator.OceanicMultiplier.ToString() + "x";
    }
    public void HandleGaiaMultiplier()
    {
        SystemGenerator.GaiaMultiplier = baseMultiplier * sliderGaia.value;
        textGaiaMultiplier.text = SystemGenerator.GaiaMultiplier.ToString() + "x";
    }
    public void HandleTemperateMultiplier()
    {
        SystemGenerator.TemperateMultiplier = baseMultiplier * sliderTemperate.value;
        textTemperateMultiplier.text = SystemGenerator.TemperateMultiplier.ToString() + "x";
    }
    public void HandleDesertMultiplier()
    {
        SystemGenerator.DesertMultiplier = baseMultiplier * sliderDesert.value;
        textDesertMultiplier.text = SystemGenerator.DesertMultiplier.ToString() + "x";
    }
    public void HandleVolcanicMultiplier()
    {
        SystemGenerator.VolcanicMultiplier = baseMultiplier * sliderVolcanic.value;
        textVolcanicMultiplier.text = SystemGenerator.VolcanicMultiplier.ToString() + "x";
    }
    public void HandleToxicMultiplier()
    {
        SystemGenerator.ToxicMultiplier = baseMultiplier * sliderToxic.value;
        textToxicMultiplier.text = SystemGenerator.ToxicMultiplier.ToString() + "x";
    }
    public void HandleBarrenMultiplier()
    {
        SystemGenerator.BarrenMultiplier = baseMultiplier * sliderBarren.value;
        textBarrenMultiplier.text = SystemGenerator.BarrenMultiplier.ToString() + "x";
    }
    public void HandleGasGiantMultiplier()
    {
        SystemGenerator.GasGiantMultiplier = baseMultiplier * sliderGasGiant.value;
        textGasGiantMultiplier.text = SystemGenerator.GasGiantMultiplier.ToString() + "x";
    }

    // Buttons 
    public void RandomizeSeed()
    {
        inputFieldSeed.text = rand.Next(0, 999999999).ToString();
    }
    public void StartNewGame()
    {
        GameController.SetGameState(GameState.Play);
        GalaxyGenerator.Instance.ExecuteGenerate(false);
    }

    // Update UI if GenerateRandom from Console.cs, etc
    private void UpdateAfterGenerate()
    {
        inputFieldSeed.text = GalaxyGenerator.Instance.Seed.ToString();
        dropDownShapeType.value = (int)GalaxyGenerator.Instance.shapeType;
        dropDownSizeType.value = (int)GalaxyGenerator.Instance.sizeType;
        dropDownNumArms.value = GalaxyGenerator.Instance.NumArms - 2; // 2 is minumum amount of arms possible
    }
    private void UpdateDropDownNumArmsOnChangeSizeType(int value)
    {
        int initialValue = dropDownNumArms.value;

        List<string> optionList = new List<string>();

        if (value == (int)GalaxyGenerator.SizeType.Tiny)
        {
            optionList.Add("2 Arms");
        }
        else if (value == (int)GalaxyGenerator.SizeType.Small)
        {
            optionList.Add("2 Arms");
            optionList.Add("3 Arms");
        }
        else if (value == (int)GalaxyGenerator.SizeType.Medium)
        {
            optionList.Add("2 Arms");
            optionList.Add("3 Arms");
            optionList.Add("4 Arms");
        }
        else if (value == (int)GalaxyGenerator.SizeType.Large)
        {
            optionList.Add("2 Arms");
            optionList.Add("3 Arms");
            optionList.Add("4 Arms");
            optionList.Add("5 Arms");
        }
        else if (value == (int)GalaxyGenerator.SizeType.Huge)
        {
            optionList.Add("2 Arms");
            optionList.Add("3 Arms");
            optionList.Add("4 Arms");
            optionList.Add("5 Arms");
            optionList.Add("6 Arms");
        }

        dropDownNumArms.options.Clear();

        // SET DROPDOWN OPTIONS
        foreach (string option in optionList)
        {
            dropDownNumArms.options.Add(new TMP_Dropdown.OptionData(option));
        }

        dropDownNumArms.value = Mathf.Clamp(initialValue, 0, dropDownNumArms.options.Count() - 1);
        dropDownNumArms.RefreshShownValue();
    }

    // Utility
    private void OnStart()
    {
        rand = new System.Random();

        // Set Dropdown Defaults
        dropDownSizeType.value = 2; // Medium
        dropDownShapeType.value = 0; // Spiral
        dropDownNumArms.value = 2; // 4 Arms

        // Set InputField Defaults
        inputFieldSeed.text = rand.Next(0, 999999999).ToString();

        // Set Slider Defaults
        SetDefaultSliderValues();

        // Set Blocker Defaults
        BlockerArms.SetActive(false); // if Spiral default, blocker false

        // Events
        GalaxyGenerator.AfterGenerate.AddListener(UpdateAfterGenerate);
    }
    public void SetDefaultSliderValues()
    {
        // Satellite SurfaceType Multiplier
        int value = 4;

        sliderIce.value = value;
        HandleIceMultiplier();

        sliderOceanic.value = value;
        HandleOceanicMultiplier();

        sliderGaia.value = value;
        HandleGaiaMultiplier();

        sliderTemperate.value = value;
        HandleTemperateMultiplier();

        sliderDesert.value = value;
        HandleDesertMultiplier();

        sliderVolcanic.value = value;
        HandleVolcanicMultiplier();

        sliderToxic.value = value;
        HandleToxicMultiplier();

        sliderBarren.value = value;
        HandleBarrenMultiplier();

        sliderGasGiant.value = value;
        HandleGasGiantMultiplier();
    }
}

