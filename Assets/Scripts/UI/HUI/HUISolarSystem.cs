using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Controls Hover User Interface (HUI) for SolarSystem objects in Galaxy View

public class HUISolarSystem : MonoBehaviour
{
    // Panel SolarSystem
    public RectTransform PanelSolarSystem;
    private float panelWidth = 150;
    private float panelHeight = 350;

    public RectTransform Line;
    float lineOffset = -25;

    public RectTransform Outline;
    public RectTransform Fill;
    float outlineFillOffset = -100;

    public TextMeshProUGUI nameText;
    public TextMeshProUGUI planetAmountText;

    private void Start()
    {
        OnStart();
    }

    private void OnEnable()
    {
        ResetMask();
        UpdateHUISolarSystem();
    }

    private void Update()
    {
        UpdateMask();
        UpdateHUISolarSystem();
    }

    // Mask
    private void UpdateMask()
    {
        if (PanelSolarSystem.gameObject.activeSelf)
        {
            if (PanelSolarSystem.sizeDelta.y < panelHeight)
            {
                float maskSpeed = panelHeight / 15f;
                PanelSolarSystem.sizeDelta = new Vector2(panelWidth, PanelSolarSystem.sizeDelta.y + maskSpeed);
            }
        }
    }
    private void ResetMask()
    {
        PanelSolarSystem.sizeDelta = new Vector2(panelWidth, 0);
    }

    // Panel
    private void UpdateHUISolarSystem()
    {
        if (InputManager.HoverUnit != null && InputManager.HoverUnit.TryGetComponent(out SolarSystem solarSystem))
        {
            UpdatePosition();
            UpdateText(solarSystem);
        }
    }

    // Position
    private void UpdatePosition()
    {
        // Get Position
        Vector3 screenPosition = PlayerCamera.Cam.WorldToScreenPoint(InputManager.HoverUnit.transform.position);

        // Set Position
        PanelSolarSystem.transform.position = screenPosition;

        SetLinePanelPosition();
    }
    private void SetLinePanelPosition()
    {
        float heightBarrier = Screen.height * 0.25f;

        int multiplier = 1;

        if (PlayerCamera.Cam.WorldToScreenPoint(InputManager.HoverUnit.transform.position).y < heightBarrier)
        {
            multiplier = -1;
        }

        // Panel
        Outline.anchoredPosition = new Vector3(0, outlineFillOffset * multiplier, 0);
        Fill.anchoredPosition = new Vector3(0, outlineFillOffset * multiplier, 0);

        // Line
        Line.anchoredPosition = new Vector3(0, lineOffset * multiplier, 0);
    } // Set LineOutlineFill, adjusts to prevent HUI from going off-screen

    // Text
    private void UpdateText(SolarSystem solarSystem)
    {
        nameText.text = solarSystem.Name;
        planetAmountText.text = solarSystem.CentralBody.SatelliteList.Count.ToString();
    }

    // Change SubView
    private void OnChangeSubView()
    {
        if (ViewController.SubViewType == SubViewType.Low)
        {
            RevealSolarSystemPanel();
        }
        else
        {
            HideSolarSystemPanel();
        }
    }

    // SolarSystem Panel
    private void RevealSolarSystemPanel()
    {
        ResetMask();
        PanelSolarSystem.gameObject.SetActive(true);
    }
    private void HideSolarSystemPanel()
    {
        PanelSolarSystem.gameObject.SetActive(false);
    }

    // Utility
    private void OnStart()
    {
        ViewController.ChangeSubView.AddListener(OnChangeSubView);
    }
}
