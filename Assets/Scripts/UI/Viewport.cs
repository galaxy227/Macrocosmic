using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewport : MonoBehaviour
{
    public static Viewport Instance
    {
        get { return instance; }
    }
    private static Viewport instance;

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
    }

    // Buttons
    public void ReturnToGalaxy()
    {
        ViewController.SetGalaxyView(InputManager.SelectedSolarSystem);
    }

    // Events
    public void OnChangeView()
    {
        if (ViewController.ViewType == ViewType.Galaxy)
        {
            gameObject.SetActive(false);
        }
        else if (ViewController.ViewType == ViewType.System)
        {
            gameObject.SetActive(true);
        }
    }
}
