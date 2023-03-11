using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InputManager : MonoBehaviour
{
    public static bool IsKBShortcutAllowed;

    // Click
    public static SolarSystem SelectedSolarSystem;
    public static Unit SelectedUnit;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float distanceBetweenClicks;
    private float distanceThreshold;

    public static UnityEvent ClickUnit;

    // Hover
    public static Unit HoverUnit;

    // Closest
    public static Unit ClosestUnit
    {
        get { return GetClosestUnit(); }
    }

    // Events
    public static UnityEvent EnterHoverUnit;
    public static UnityEvent ExitHoverUnit;

    private void Awake()
    {
        OnAwake();
    }
    void Start()
    {
        OnStart();
    }
    void Update()
    {
        HandleClick();
        HandleHover();
        HandleKBShortcut();
    }

    // Click
    private void HandleClick()
    {
        // Differentiate between click or drag by comparing Vector3 mouse position at start and end of click
        distanceThreshold = 5; // pixels

        if (!Tools.IsMouseOverUI())
        {
            if (Input.GetMouseButtonDown(0))
            {
                startPosition = Input.mousePosition;
            }

            if (Input.GetMouseButtonUp(0))
            {
                endPosition = Input.mousePosition;
                distanceBetweenClicks = Vector3.Distance(startPosition, endPosition);

                if (distanceBetweenClicks < distanceThreshold)
                {
                    Vector3 origin = new Vector3(PlayerCamera.Instance.MinCursorPosition.x, PlayerCamera.Instance.MinCursorPosition.y, PlayerCamera.Instance.MaxZoom);
                    RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.back, PlayerCamera.Instance.MaxZoom);

                    if (hit.collider != null)
                    {
                        if (hit.collider.gameObject.TryGetComponent(out SolarSystem system)) // Solar System
                        {
                            if (ViewController.SubViewType == SubViewType.Low)
                            {
                                SelectedSolarSystem = system;

                                ViewController.SetSystemView(SelectedSolarSystem);
                            }
                        }
                        else if (hit.collider.gameObject.TryGetComponent(out Unit unit)) // Other
                        {
                            SelectedUnit = unit;

                            ClickUnit.Invoke();
                        }
                    }
                }
            }
        }
    }

    // Hover
    private void HandleHover()
    {
        if (!Tools.IsMouseOverUI() && !PlayerCamera.Instance.IsDragging && !PlayerCamera.Instance.IsKeyDragging)
        {
            Vector3 origin = new Vector3(PlayerCamera.Instance.MinCursorPosition.x, PlayerCamera.Instance.MinCursorPosition.y, PlayerCamera.Instance.MaxZoom);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector3.back, PlayerCamera.Instance.MaxZoom);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.TryGetComponent(out Unit unit)) // Other
                {
                    if (unit != HoverUnit)
                    {
                        HoverUnit = unit;

                        EnterHoverUnit.Invoke();
                    }
                }
                else
                {
                    OnExitHoverUnit();
                }
            }
            else
            {
                OnExitHoverUnit();
            }
        }
        else
        {
            OnExitHoverUnit();
        }
    }
    private void OnExitHoverUnit()
    {
        HoverUnit = null;

        ExitHoverUnit.Invoke();
    }

    // Closest Unit
    private static Unit GetClosestUnit()
    {
        Unit unitToReturn = null;
        float pixelThreshold = Screen.width * 0.1f;
        float minPixelDistance = Mathf.Infinity;

        if (ViewController.ViewType == ViewType.System)
        {
            // Create List
            List<Unit> unitList = new List<Unit>();

            unitList.Add(SelectedSolarSystem.centralBody);

            foreach (Satellite satellite in SelectedSolarSystem.satelliteList)
            {
                unitList.Add(satellite);
            }

            // Get Closest Unit
            foreach (Unit unit in unitList)
            {
                if (unit.GetComponent<Renderer>().isVisible)
                {
                    float pixelDistance = Vector3.Distance(PlayerCamera.Cam.WorldToScreenPoint(unit.transform.position), Input.mousePosition);

                    if (pixelDistance < minPixelDistance)
                    {
                        minPixelDistance = pixelDistance;
                        unitToReturn = unit;
                    }
                }
            }
        }

        return unitToReturn;
    }

    // KB Shortcuts
    private void HandleKBShortcut()
    {
        if (!InputFieldController.IsInputFieldSelected)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UIManager.Instance.HandleEscape();
            }

            if (GameController.GameState == GameState.Play)
            {
                if (IsKBShortcutAllowed)
                {
                    // Change View
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if (ViewController.ViewType == ViewType.Galaxy)
                        {
                            ViewController.SetSystemView(SelectedSolarSystem);
                        }
                        else if (ViewController.ViewType == ViewType.System)
                        {
                            ViewController.SetGalaxyView(SelectedSolarSystem);
                        }
                    }

                    // Change Time
                    if (Input.GetKeyDown(KeyCode.Plus) || Input.GetKeyDown(KeyCode.KeypadPlus)) // Increase
                    {
                        TimeController.Instance.IncrementSpeedType(true);
                    }
                    else if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.KeypadMinus)) // Decrease
                    {
                        TimeController.Instance.IncrementSpeedType(false);
                    }
                    else if (Input.GetKeyDown(KeyCode.Space)) // Pause
                    {
                        TimeController.Instance.TogglePause();
                    }
                }

                // Toggle Console
                if (Input.GetKeyDown(KeyCode.Tilde) || Input.GetKeyDown(KeyCode.BackQuote)) 
                {
                    UIManager.Instance.ToggleConsole();
                }
            }
        }
    }

    // Utility
    private void OnAwake()
    {
        IsKBShortcutAllowed = true;

        if (ClickUnit == null)
        {
            ClickUnit = new UnityEvent();
        }

        if (EnterHoverUnit == null)
        {
            EnterHoverUnit = new UnityEvent();
        }

        if (ExitHoverUnit == null)
        {
            ExitHoverUnit = new UnityEvent();
        }
    }
    private void OnStart()
    {
        GalaxyGenerator.BeforeGenerate.AddListener(OnGenerateGalaxy);
    }
    private void OnGenerateGalaxy()
    {
        SelectedSolarSystem = null;
    }
}