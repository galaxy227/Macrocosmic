using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Used for Load/Settings to change to different colors when enabled by MainMenu or MiniMenu

public class PanelAlphaHelper : MonoBehaviour
{
    public Color MainMenuColor;
    public Color MiniMenuColor;
    private Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }
    private void OnEnable()
    {
        if (GameController.GameState == GameState.MainMenu) // MainMenu
        {
            image.color = MainMenuColor;
        }
        else if (GameController.GameState == GameState.Play) // MiniMenu
        {
            image.color = MiniMenuColor;
        }
    }
}
