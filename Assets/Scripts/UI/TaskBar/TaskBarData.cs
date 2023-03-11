using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class TaskBarData : MonoBehaviour
{
    // Info
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI SeedText;
    public TextMeshProUGUI SizeText;
    public TextMeshProUGUI ShapeText;
    // Stars
    public TextMeshProUGUI StarsText;
    public TextMeshProUGUI RedStarText; 
    public TextMeshProUGUI OrangeStarText;
    public TextMeshProUGUI YellowStarText;
    public TextMeshProUGUI WhiteStarText;
    public TextMeshProUGUI BlueStarText;
    public TextMeshProUGUI BlackHoleText;
    // Planets
    public TextMeshProUGUI PlanetsText;
    public TextMeshProUGUI IceText; 
    public TextMeshProUGUI OceanicText;
    public TextMeshProUGUI GaiaText;
    public TextMeshProUGUI TemperateText;
    public TextMeshProUGUI DesertText;
    public TextMeshProUGUI VolcanicText;
    public TextMeshProUGUI ToxicText;
    public TextMeshProUGUI BarrenText;
    public TextMeshProUGUI GasGiantText;

    private void OnEnable()
    {
        UpdateText();
    }

    // Text
    public void UpdateText()
    {
        // Info
        if (GalaxyGenerator.Instance != null)
        {
            NameText.text = GalaxyGenerator.Instance.Name;
            SeedText.text = GalaxyGenerator.Instance.Seed.ToString();
            SizeText.text = GalaxyGenerator.Instance.sizeType.ToString();
            ShapeText.text = GalaxyGenerator.Instance.shapeType.ToString();
        }

        // Stars
        StarsText.text = Counter.SystemCount.ToString();
        RedStarText.text = Counter.RedCount.ToString();
        OrangeStarText.text = Counter.OrangeCount.ToString();
        YellowStarText.text = Counter.YellowCount.ToString();
        WhiteStarText.text = Counter.WhiteCount.ToString();
        BlueStarText.text = Counter.BlueCount.ToString();
        BlackHoleText.text = Counter.BlackHoleCount.ToString();

        // Planets
        PlanetsText.text = Counter.PlanetCount.ToString();
        IceText.text = Counter.IceCount.ToString();
        OceanicText.text = Counter.OceanicCount.ToString();
        GaiaText.text = Counter.GaiaCount.ToString();
        TemperateText.text = Counter.TemperateCount.ToString();
        DesertText.text = Counter.DesertCount.ToString();
        VolcanicText.text = Counter.VolcanicCount.ToString();
        ToxicText.text = Counter.ToxicCount.ToString();
        BarrenText.text = Counter.BarrenCount.ToString();
        GasGiantText.text = Counter.GasGiantCount.ToString();
    }
}

