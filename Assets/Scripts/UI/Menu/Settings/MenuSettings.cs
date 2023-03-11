using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuSettings : MonoBehaviour
{
    [Header("Camera")]
    // Zoom Speed
    public TextMeshProUGUI zoomText;
    public Slider zoomSlider;
    private const float zoomDefault = 0.5f;
    private const string zoomReference = "ZoomSpeed";
    // Movement Speed
    public TextMeshProUGUI movementText;
    public Slider movementSlider;
    private const float movementDefault = 0.5f;
    private const string movementReference = "MovementSpeed";

    public bool isPopUpCancel; // do not update menusettings if cancelling popup

    public static MenuSettings Instance
    {
        get { return instance; }
    }
    private static MenuSettings instance;
    private int enableCount;

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

        enableCount = 0;
    }
    private void OnEnable()
    {
        if (enableCount > 0 && !isPopUpCancel)
        {
            SetValues();
            UpdateText();
        }

        isPopUpCancel = false;
        enableCount++;
    }

    private void SetValues()
    {
        // Zoom
        if (!PlayerPrefs.HasKey(zoomReference))
        {
            PlayerPrefs.SetFloat(zoomReference, zoomDefault);
        }

        zoomSlider.value = PlayerPrefs.GetFloat(zoomReference);

        // Movement
        if (!PlayerPrefs.HasKey(movementReference))
        {
            PlayerPrefs.SetFloat(movementReference, movementDefault);
        }

        movementSlider.value = PlayerPrefs.GetFloat(movementReference);
    }
    public void UpdateText()
    {
        // Camera
        zoomText.text = System.Math.Round(zoomSlider.value * 100f, 0).ToString();
        movementText.text = System.Math.Round(movementSlider.value * 100f, 0).ToString();
    }

    // Buttons
    public void ApplySettings()
    {
        // Zoom
        PlayerPrefs.SetFloat(zoomReference, zoomSlider.value);
        PlayerCamera.Instance.UserZoomSpeed = zoomSlider.value;

        // Movement
        PlayerPrefs.SetFloat(movementReference, movementSlider.value);
        PlayerCamera.Instance.UserKeySpeed = movementSlider.value;

        UIManager.Instance.BackToLastActiveCanvas();
    }
    public void SetDefaultSettings()
    {
        // Camera
        PlayerPrefs.SetFloat(zoomReference, zoomDefault);
        PlayerPrefs.SetFloat(movementReference, movementDefault);
    }
}