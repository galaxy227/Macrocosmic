using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls Hover User Interfaces (HUI)

public class HUIController : MonoBehaviour
{
    // HUI Canvas
    public Canvas SolarSystem;

    void Start()
    {
        OnStart();
    }

    // Event Methods
    private void OnEnterHover()
    {
        if (InputManager.HoverUnit.TryGetComponent(out SolarSystem system))
        {
            RevealHUI(SolarSystem);
        }
    }
    private void OnExitHover()
    {
        HideAllHUI();
    }

    // Reveal & Hide
    private void RevealHUI(Canvas HUI)
    {
        HUI.gameObject.SetActive(true);
    }
    private void HideAllHUI()
    {
        SolarSystem.gameObject.SetActive(false);
    }

    // Utility
    private void OnStart()
    {
        HideAllHUI();

        // Events
        InputManager.EnterHoverUnit.AddListener(OnEnterHover);
        InputManager.ExitHoverUnit.AddListener(OnExitHover);
    }
}
