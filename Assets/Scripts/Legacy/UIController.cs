using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Controls all Canvas

public class UIController : MonoBehaviour
{
    public static UIController Instance
    {
        get {  return instance; }
    }
    private static UIController instance;

    // Canvas
    public Canvas Console;
    public Canvas TimeBar;
    public Canvas ViewPort;
    public Canvas TaskBar;
    public Canvas MiniMenu;

    public static bool IsInputFieldSelected;

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
    private void OnEnable()
    {
        InitializeUIController();
    }

    private void Update()
    {
        OpenConsole();
    }

    // KB Shortcuts
    private void OpenConsole()
    {
        if (Input.GetKeyDown(KeyCode.Tilde) || Input.GetKeyDown(KeyCode.BackQuote)) // Open Console
        {
            if (Console.gameObject.activeSelf)
            {
                Console.gameObject.SetActive(false);
            }
            else if (!Console.gameObject.activeSelf)
            {
                Console.gameObject.SetActive(true);
            }
        }
    }

    // Event Methods
    private void OnChangeView()
    {
        if (ViewController.ViewType == ViewType.Galaxy)
        {
            ViewPort.gameObject.SetActive(false);
        }
        else if (ViewController.ViewType == ViewType.System)
        {
            ViewPort.gameObject.SetActive(true);
        }
    }
    private void OnChangeGameState()
    {
        if (GameController.GameState == GameState.Play)
        {
            gameObject.SetActive(true);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    // Utility
    private void OnStart()
    {
        gameObject.SetActive(false);

        //TaskBar.GetComponent<TaskBarData>().Initialize();

        GameController.ChangeGameState.AddListener(OnChangeGameState);
        ViewController.ChangeView.AddListener(OnChangeView);
    }
    private void InitializeUIController()
    {
        Console.gameObject.SetActive(false);

        if (ViewController.ViewType == ViewType.Galaxy)
        {
            ViewPort.gameObject.SetActive(false);
        }
    }
}

//// Event Trigger, Scroll
//public void OnPointerEnterScroll(GameObject obj) 
//{
//    if (obj.TryGetComponent(out Image image))
//    {
//        if (obj.TryGetComponent(out Scrollbar scrollBar)) // background
//        {
//            image.color = new Color(image.color.r, image.color.g, image.color.b, 5 / 255f);
//        }
//        else // handle
//        {
//            image.color = new Color(image.color.r, image.color.g, image.color.b, 15 / 255f);
//        }
//    }
//}
//public void OnPointerExitScroll(GameObject obj)
//{
//    if (obj.TryGetComponent(out Image image))
//    {
//        image.color = new Color(image.color.r, image.color.g, image.color.b, 0);
//    }
//}
//// Event Trigger, SolarSystem
//public void OnPointerEnterSystemSprite(GameObject obj) 
//{
//    obj.SetActive(true);
//}
//public void OnPointerExitSystemSprite(GameObject obj)
//{
//    obj.SetActive(false);
//}
