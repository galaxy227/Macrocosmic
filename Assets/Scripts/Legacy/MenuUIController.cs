using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIController : MonoBehaviour
{
    public static MenuUIController Instance
    {
        get {  return instance; }
    }
    private static MenuUIController instance;

    // Canvas
    public Canvas MainMenu;
    public Canvas New;
    public Canvas Load;
    public Canvas Settings;

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
    private void Start()
    {
        OnStart();
    }

    // Canvas
    public void OpenCanvas(Canvas canvas)
    {
        CloseAllCanvas();
        canvas.gameObject.SetActive(true);
    }
    public void BackToMenu()
    {
        if (GameController.GameState == GameState.MainMenu) // MainMenu
        {
            OpenCanvas(MainMenu);
        }
        else if (GameController.GameState == GameState.Play) // MiniMenu
        {
            CloseAllCanvas();
            gameObject.SetActive(false);
            //MiniMenu.Instance.PanelMiniMenu.gameObject.SetActive(true);
        }
    }
    private void CloseAllCanvas()
    {
        MainMenu.gameObject.SetActive(false);
        New.gameObject.SetActive(false);
        Load.gameObject.SetActive(false);
        Settings.gameObject.SetActive(false);
    }

    // Event Methods
    private void OnChangeGameState()
    {
        if (GameController.GameState == GameState.MainMenu)
        {
            OpenCanvas(MainMenu);
            gameObject.SetActive(true);
        }
        else
        {
            OpenCanvas(MainMenu);
            gameObject.SetActive(false);
        }
    }

    // Utility
    private void OnStart()
    {
        OpenCanvas(MainMenu);
        GameController.ChangeGameState.AddListener(OnChangeGameState);
    }
}
