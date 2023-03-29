using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.EventSystems;

public enum CanvasType
{
    Blocker,
    Debug,
    Static,
    PopUp,
    TaskBar,
    Menu,
    Unit,
}

public class UIManager : MonoBehaviour
{
    public Canvas BlockerCanvas;
    public Canvas ConsoleCanvas;
    [Header("Static")]
    public Canvas TimeBarCanvas; // GameState.Play
    public Canvas TaskBarCanvas;
    public Canvas ViewPortCanvas;
    [Header("Active")]
    public Canvas PopUpCanvas; // PopUp
    public Canvas SummaryCanvas; // TaskBar
    public Canvas MiniMenuCanvas;
    public Canvas MainMenuCanvas; // Menu
    public Canvas NewCanvas;
    public Canvas LoadCanvas;
    public Canvas SettingsCanvas;
    [Header("Unit")]
    public Canvas CentralBodyCanvas;
    public Canvas SatelliteCanvas;

    public Dictionary<Canvas, CanvasType> CanvasDictionary = new Dictionary<Canvas, CanvasType>();
    private List<Canvas> canvasList = new List<Canvas>();
    public List<Canvas> activeCanvasList = new List<Canvas>();
    private List<Canvas> unitCanvasList = new List<Canvas>();

    [HideInInspector] public Canvas CurrentActiveCanvas;
    [HideInInspector] public Canvas LastActiveCanvas
    {
        get 
        {
            if (activeCanvasList.Count >= 2)
            {
                return activeCanvasList[activeCanvasList.Count - 2];
            }
            else
            {
                return null;
            }
        }
    } // in activeCanvasList
    [HideInInspector] public Canvas PreviousActiveCanvas; // actual last
    public bool IsActiveCanvasFirstOpen;
    private bool isPauseOnOpen;

    public static UIManager Instance
    {
        get { return instance; }
    }
    private static UIManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            OnAwake();
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

    // Buttons
    public void ToggleConsole()
    {
        if (ConsoleCanvas.gameObject.activeSelf)
        {
            ConsoleCanvas.gameObject.SetActive(false);
        }
        else if (!ConsoleCanvas.gameObject.activeSelf)
        {
            ConsoleCanvas.gameObject.SetActive(true);
        }
    }
    public void OpenPopUpCanvas(GameObject PanelToOpen)
    {
        // Clear previous children
        List<GameObject> objList = Tools.GetAllChildren(PopUpCanvas.gameObject, false);

        foreach (GameObject obj in objList)
        {
            Destroy(obj);
        }

        // Instantiate PanelToOpen
        Instantiate(PanelToOpen, PopUpCanvas.gameObject.transform);

        OpenActiveCanvas(PopUpCanvas);
    } // PopUp
    public void ToggleActiveCanvas(Canvas canvas)
    {
        if (canvas != null)
        {
            bool isCanvasOpening = !canvas.gameObject.activeSelf;

            if (isCanvasOpening)
            {
                OpenActiveCanvas(canvas);

                if (CanvasDictionary[CurrentActiveCanvas] == CanvasType.TaskBar || CanvasDictionary[LastActiveCanvas] == CanvasType.TaskBar)
                {
                    TaskBarController.Instance.SetTaskBarView(CurrentActiveCanvas, true);
                }
            }
            else
            {
                DisableAllActiveCanvas();
            }
        }
        else
        {
            DisableAllActiveCanvas();
        }
    } // Active
    public void OpenActiveCanvas(Canvas canvasToOpen)
    {
        // LastActiveCanvas
        if (CurrentActiveCanvas != null)
        {
            CloseActiveCanvas(CurrentActiveCanvas);
            PreviousActiveCanvas = CurrentActiveCanvas;
        }

        // Set ActiveCanvas
        CurrentActiveCanvas = canvasToOpen;

        if (activeCanvasList.Contains(CurrentActiveCanvas))
        {
            activeCanvasList.RemoveAt(activeCanvasList.Count - 1);
            IsActiveCanvasFirstOpen = false;
        }
        else
        {
            activeCanvasList.Add(CurrentActiveCanvas);
            IsActiveCanvasFirstOpen = true;
        }

        CurrentActiveCanvas.gameObject.SetActive(true);

        // GameState Play
        if (GameController.Instance.GameState == GameState.Play)
        {
            // Unit canvas
            DisableAllUnitCanvas();

            // Time & KB
            if (TimeController.Instance.SpeedType != SpeedType.Paused)
            {
                TimeController.Instance.TogglePause();
                isPauseOnOpen = false;
            }
            else
            {
                if (LastActiveCanvas == null)
                {
                    isPauseOnOpen = true;
                }
            }
        }

        InputManager.IsKBShortcutAllowed = false;
    }
    public void BackToLastActiveCanvas()
    {
        if (LastActiveCanvas != null)
        {
            OpenActiveCanvas(LastActiveCanvas);
        }
    }
    public void OpenUnitCanvas()
    {
        if (GameController.Instance.GameState == GameState.Play && InputManager.SelectedUnit != null)
        {
            if (InputManager.SelectedUnit is CentralBody && (!CentralBodyCanvas.gameObject.activeSelf  || InputManager.SelectedUnit != CentralBodyCanvas.GetComponent<UICentralBody>().Unit))
            {
                DisableAllUnitCanvas();
                CentralBodyCanvas.gameObject.SetActive(true);
            }
            else if (InputManager.SelectedUnit is Satellite && (!SatelliteCanvas.gameObject.activeSelf || InputManager.SelectedUnit != SatelliteCanvas.GetComponent<UISatellite>().Unit))
            {
                DisableAllUnitCanvas();
                SatelliteCanvas.gameObject.SetActive(true);
            }
        }
    } // Unit
    public void CloseUnitCanvas()
    {
        DisableAllUnitCanvas();
        InputManager.SelectedUnit = null;
    }

    // KB
    public void HandleEscape()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if (activeCanvasList.Count > 0) // Close current canvas
        {
            if (activeCanvasList.Count > 1)
            {
                if (CanvasDictionary[CurrentActiveCanvas] == CanvasType.TaskBar)
                {
                    ToggleActiveCanvas(CurrentActiveCanvas);
                }
                else
                {
                    BackToLastActiveCanvas();
                }
            }
            else
            {
                if (GameController.Instance.GameState == GameState.Play)
                {
                    DisableAllActiveCanvas();
                }
            }
        }
        else // Open mini menu
        {
            if (GameController.Instance.GameState == GameState.Play)
            {
                ToggleActiveCanvas(MiniMenuCanvas);
            }
        }
    }

    // Presets
    private void SetUI()
    {
        if (GameController.Instance.GameState == GameState.MainMenu)
        {
            SetMainMenu();
        }
        else if (GameController.Instance.GameState == GameState.Play)
        {
            SetPlay();
        }
    }
    private void SetPlay()
    {
        DisableAllCanvas();

        SetAllStaticCanvas(true);
    }
    private void SetMainMenu()
    {
        DisableAllCanvas();

        OpenActiveCanvas(MainMenuCanvas);
    }
    // Preset Helper
    public void SetAllStaticCanvas(bool isActive)
    {
        if (GameController.Instance.GameState == GameState.Play)
        {
            TimeBarCanvas.gameObject.SetActive(isActive);
            TaskBarCanvas.gameObject.SetActive(isActive);

            if (ViewController.ViewType == ViewType.System)
            {
                ViewPortCanvas.gameObject.SetActive(isActive);
            }
        }
    }

    // Events
    private void OnChangeView()
    {
        if (GameController.Instance.GameState == GameState.Play)
        {
            if (ViewController.ViewType == ViewType.Galaxy)
            {
                ViewPortCanvas.gameObject.SetActive(false);
                CloseUnitCanvas();
            }
            else if (ViewController.ViewType == ViewType.System)
            {
                ViewPortCanvas.gameObject.SetActive(true);
            }
        }
    }

    // Utility
    private void OnAwake()
    {
        // Set CanvasTypeDictionary
        CanvasDictionary[BlockerCanvas] = CanvasType.Blocker; // Blocker
        CanvasDictionary[ConsoleCanvas] = CanvasType.Debug; // Debug
        CanvasDictionary[TimeBarCanvas] = CanvasType.Static; // Static
        CanvasDictionary[TaskBarCanvas] = CanvasType.Static;
        CanvasDictionary[ViewPortCanvas] = CanvasType.Static;
        CanvasDictionary[PopUpCanvas] = CanvasType.PopUp; // PopUp
        CanvasDictionary[SummaryCanvas] = CanvasType.TaskBar; // TaskBar
        CanvasDictionary[MiniMenuCanvas] = CanvasType.TaskBar;
        CanvasDictionary[MainMenuCanvas] = CanvasType.Menu; // Menu
        CanvasDictionary[NewCanvas] = CanvasType.Menu;
        CanvasDictionary[LoadCanvas] = CanvasType.Menu;
        CanvasDictionary[SettingsCanvas] = CanvasType.Menu;
        CanvasDictionary[CentralBodyCanvas] = CanvasType.Unit; // Unit
        CanvasDictionary[SatelliteCanvas] = CanvasType.Unit;

        // Set canvasList
        canvasList.Add(BlockerCanvas); // Blocker
        canvasList.Add(ConsoleCanvas); // Debug
        canvasList.Add(TimeBarCanvas); // Static
        canvasList.Add(TaskBarCanvas);
        canvasList.Add(ViewPortCanvas);
        canvasList.Add(PopUpCanvas); // PopUp
        canvasList.Add(SummaryCanvas); // TaskBar
        canvasList.Add(MiniMenuCanvas);
        canvasList.Add(MainMenuCanvas); // MainMenu
        canvasList.Add(NewCanvas);
        canvasList.Add(LoadCanvas);
        canvasList.Add(SettingsCanvas);
        canvasList.Add(CentralBodyCanvas); // Unit
        canvasList.Add(SatelliteCanvas);

        // Set unitCanvasList
        unitCanvasList.Add(CentralBodyCanvas); // Unit
        unitCanvasList.Add(SatelliteCanvas);
    }
    private void OnStart()
    {
        SetUI();

        // Events
        GalaxyGenerator.BeforeGenerate.AddListener(DisableAllUnitCanvas);
        GameController.ChangeGameState.AddListener(SetUI); // GameState
        InputManager.ClickUnit.AddListener(OpenUnitCanvas); // Unit
        ViewController.ChangeView.AddListener(OnChangeView); // ViewController
    }
    private void DisableAllCanvas()
    {
        DisableAllActiveCanvas();
        DisableAllUnitCanvas();

        foreach (Canvas canvas in canvasList)
        {
            if (canvas.gameObject != null)
            {
                canvas.gameObject.SetActive(false);
            }
        }
    } // Canvas
    private void DisableAllUnitCanvas()
    {
        foreach (Canvas canvas in unitCanvasList)
        {
            canvas.gameObject.SetActive(false);
        }
    } // Unit
    public void DisableAllActiveCanvas()
    {
        // Close all active canvases
        for (int i = activeCanvasList.Count - 1; i >= 0; i--)
        {
            DisableActiveCanvas(activeCanvasList[i]);
        }

        // GameState Play
        if (GameController.Instance.GameState == GameState.Play)
        {
            // Unit canvas
            if (ViewController.ViewType == ViewType.System)
            {
                OpenUnitCanvas();
            }
            else
            {
                CloseUnitCanvas();
            }

            // Time & KB
            if (!isPauseOnOpen)
            {
                TimeController.Instance.TogglePause();
            }
        }

        InputManager.IsKBShortcutAllowed = true;
    } // Active
    private void DisableActiveCanvas(Canvas activeCanvas)
    {
        if (CanvasDictionary[activeCanvas] == CanvasType.TaskBar)
        {
            TaskBarController.Instance.SetTaskBarView(activeCanvas, false);
        }

        CloseActiveCanvas(activeCanvas);
        activeCanvasList.Remove(activeCanvas);

        if (activeCanvas == CurrentActiveCanvas)
        {
            CurrentActiveCanvas = null;
        }
    }
    private void CloseActiveCanvas(Canvas activeCanvas)
    {
        activeCanvas.gameObject.SetActive(false);
    }
}
