using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UnitUIController : MonoBehaviour
{
    // Unit Canvas
    public UICentralBody CentralBody;
    public UISatellite Satellite;

    void Start()
    {
        OnStart();
    }
    private void OnEnable()
    {
        SetAllCanvasFalse();
    }

    // Event Methods
    private void OpenUnitPanel()
    {
        if (InputManager.SelectedUnit is CentralBody)
        {
            SetAllCanvasFalse();
            CentralBody.gameObject.SetActive(true);
        }
        else if (InputManager.SelectedUnit is Satellite)
        {
            SetAllCanvasFalse();
            Satellite.gameObject.SetActive(true);
        }
    }
    private void OnChangeGameState()
    {
        if (GameController.Instance.GameState == GameState.Play)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Buttons
    public void CloseUnitPanel()
    {
        SetAllCanvasFalse();

        InputManager.SelectedUnit = null;
    }

    // Utility
    private void OnStart()
    {
        gameObject.SetActive(false);

        InputManager.ClickUnit.AddListener(OpenUnitPanel);
        ViewController.ChangeView.AddListener(CloseUnitPanel);
        GalaxyGenerator.BeforeGenerate.AddListener(CloseUnitPanel);
        GameController.ChangeGameState.AddListener(OnChangeGameState);
    }
    private void SetAllCanvasFalse()
    {
        CentralBody.gameObject.SetActive(false);
        Satellite.gameObject.SetActive(false);
    }
}
