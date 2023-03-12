using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class PlayerCamera : MonoBehaviour
{
    // Lock
    public GameObject Anchor;
    private Unit lockedUnit;

    // Settings
    [Range(0f, 1f)] public float UserZoomSpeed;
    private float zoomInSpeed;
    private float zoomOutSpeed;

    [Range(0f, 1f)] public float UserKeySpeed;
    private float keySpeed;

    public float MaxZoom;
    public float MinZoom = 0;
    public float MaxPan;
    public float ZoomBarrier;

    public float LerpSpeed;

    // Camera
    public Vector3 localTargetPosition;
    private Vector3 localPosition;

    private Vector3 minCursorPosition;
    private Vector3 maxCursorPosition;

    // Zoom
    private Vector3 worldZoomTarget;
    private float zoomPercentage; // not used for calculating targetPosition
    private bool isZooming;

    // Drag
    private Vector3 localDragTarget;
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private bool isDragging;
    private bool isMouseOverUIAtDragStartPosition;

    // KeyDrag
    private Vector3 localKeyDragTarget;
    private bool isKeyDragging;

    // Events
    public static UnityEvent AfterCamera;

    // Reference
    public static PlayerCamera Instance;
    public static Camera Cam;
    public Vector3 MinCursorPosition
    {
        get { return minCursorPosition; }
    }
    public Vector3 MaxCursorPosition
    {
        get { return maxCursorPosition; }
    }
    public Unit LockedUnit
    {
        get { return lockedUnit; }
    }
    public float ZoomPercentage
    {
        get { return zoomPercentage; }
    }
    public bool IsZooming
    {
        get { return isZooming; }
    }
    public bool IsDragging
    {
        get { return isDragging; }
    }
    public bool IsKeyDragging
    {
        get { return isKeyDragging; }
    }

    private void Awake()
    {
        OnAwake();
    }
    void Start()
    {
        OnStart();
    }

    void LateUpdate()
    {
        SetValues();

        if (GameController.Instance.GameState == GameState.Play)
        {
            HandleInput();
            UpdatePosition();
        }

        AfterCamera.Invoke();
    }

    private void UpdatePosition()
    {
        // Set localPosition
        if (isDragging)
        {
            localTargetPosition = localDragTarget;
        }
        else if (isKeyDragging)
        {
            localTargetPosition = localKeyDragTarget;
        }
        else if (isZooming)
        {
            localTargetPosition = Anchor.transform.InverseTransformPoint(worldZoomTarget);
        }

        // Set worldPosition from localPosition
        Vector3 worldTargetPosition = Anchor.transform.TransformPoint(localTargetPosition);

        // Move Camera to worldPosition
        transform.position = Vector3.Lerp(transform.position, worldTargetPosition, Time.deltaTime * LerpSpeed);
        transform.position = SetBoundary(transform.position);

        localPosition = Anchor.transform.InverseTransformPoint(transform.position);
    }

    // Input
    private void HandleInput()
    {
        HandleMouseZoom();
        HandleMouseDrag();
        HandleKeyDrag();
    }
    private void HandleMouseZoom()
    {
        if (!Tools.IsMouseOverUI())
        {
            int mouseScroll = (int)Input.mouseScrollDelta.y;
            int zoomIn = 1;
            int zoomOut = -1;

            if (mouseScroll == zoomIn)
            {
                worldZoomTarget = Vector3.Lerp(transform.position, minCursorPosition, zoomInSpeed);

                isZooming = true;

                if (worldZoomTarget.z < ZoomBarrier)
                {
                    float zPercentage = Mathf.InverseLerp(worldZoomTarget.z, transform.position.z, ZoomBarrier);

                    worldZoomTarget = Vector3.Lerp(worldZoomTarget, transform.position, zPercentage);
                }
            }
            else if (mouseScroll == zoomOut)
            {
                worldZoomTarget = Vector3.Lerp(transform.position, maxCursorPosition, zoomOutSpeed);
                isZooming = true;
            }
            else
            {
                isZooming = false;
            }
        }
    }
    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPosition = Anchor.transform.InverseTransformPoint(minCursorPosition);

            isMouseOverUIAtDragStartPosition = Tools.IsMouseOverUI();
        }

        if (!isMouseOverUIAtDragStartPosition)
        {
            if (Input.GetMouseButton(0))
            {
                dragCurrentPosition = Anchor.transform.InverseTransformPoint(minCursorPosition);

                isDragging = true;
            }
            else
            {
                isDragging = false;
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 localDragOffset = dragStartPosition - dragCurrentPosition;
                Vector3 currentLocalPosition = Anchor.transform.InverseTransformPoint(transform.position);

                localDragTarget = currentLocalPosition + localDragOffset;
            }
        }
    }
    private void HandleKeyDrag()
    {
        if (!InputFieldController.IsInputFieldSelected)
        {
            Vector3 currentLocalPosition = Anchor.transform.InverseTransformPoint(transform.position);
            Vector3 localKeyDragOffset = Vector3.zero;
            isKeyDragging = false;

            // Distance
            float distanceAcrossScreen = Vector3.Distance(GetScreenLeftWorldPosition(), GetScreenRightWorldPosition());
            Vector3 distanceUp = new Vector3(0, distanceAcrossScreen, 0);
            Vector3 distanceRight = new Vector3(distanceAcrossScreen, 0, 0);

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                localKeyDragOffset += distanceUp * keySpeed;

                isKeyDragging = true;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                localKeyDragOffset += -distanceUp * keySpeed;

                isKeyDragging = true;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                localKeyDragOffset += -distanceRight * keySpeed;

                isKeyDragging = true;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                localKeyDragOffset += distanceRight * keySpeed;

                isKeyDragging = true;
            }

            if (isKeyDragging)
            {
                localKeyDragTarget = currentLocalPosition + localKeyDragOffset;
            }
        }
    }

    // Values
    private void SetValues()
    {
        // Anchor
        SetAnchorPosition();

        // Zoom
        zoomInSpeed = Mathf.Lerp(0.25f, 0.75f, UserZoomSpeed);

        float multiplier = zoomInSpeed / (1 - zoomInSpeed);
        float distance = Mathf.Abs(transform.position.z - maxCursorPosition.z);
        zoomOutSpeed = (transform.position.z * multiplier) / distance;

        zoomPercentage = Mathf.InverseLerp(ZoomBarrier, MaxZoom, transform.position.z);

        // Key
        keySpeed = Mathf.Lerp(0.01f, 0.05f, UserKeySpeed);

        // Position
        transform.position = Anchor.transform.TransformPoint(localPosition);

        // Cursor
        SetMinCursorPosition();
        SetMaxCursorPosition();
    }

    // Mouse
    private void SetMinCursorPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = transform.position.z;

        Vector3 worldPosition = Cam.ScreenToWorldPoint(mousePosition);

        minCursorPosition = worldPosition;
    }
    private void SetMaxCursorPosition()
    {
        Vector3 position = transform.position;
        Vector3 direction = (position - minCursorPosition).normalized;

        while (position.z < MaxZoom)
        {
            position = position + direction;
        }

        maxCursorPosition = position;
    }

    // Boundary
    private Vector3 SetBoundary(Vector3 position)
    {
        float MaxPanY = MaxPan * 0.55f;

        position.x = Mathf.Clamp(position.x, -MaxPan, MaxPan);
        position.y = Mathf.Clamp(position.y, -MaxPanY, MaxPanY);
        position.z = Mathf.Clamp(position.z, ZoomBarrier, MaxZoom);

        return position;
    }

    // Lock
    private void SetAnchorPosition()
    {
        // Set Locked Unit
        Unit originalUnit = lockedUnit;
        SetLockedUnit();

        // Set Anchor Position
        if (lockedUnit != null)
        {
            Anchor.transform.position = new Vector3(lockedUnit.transform.position.x, lockedUnit.transform.position.y, 0);
        }
        else
        {
            Anchor.transform.position = Vector3.zero;
        }

        // Set Local Position IF LockedUnit changes
        if (lockedUnit != originalUnit)
        {
            localPosition = Anchor.transform.InverseTransformPoint(transform.position);
        }
    }
    private void SetLockedUnit()
    {
        if (!Tools.IsMouseOverUI())
        {
            if (Input.mouseScrollDelta.y == 1) // Zoom In
            {
                // Calculate next position
                Vector3 nextZoomInPosition = Vector3.Lerp(transform.position, minCursorPosition, zoomInSpeed);
                float nextZPosition = nextZoomInPosition.z;

                if (ViewController.ViewType == ViewType.System)
                {
                    if (nextZPosition < ViewController.lowBarrierSystem) // if next position's subViewType is Low
                    {
                        lockedUnit = InputManager.ClosestUnit;
                    }
                }
            }
            else if (Input.mouseScrollDelta.y == -1) // Zoom Out
            {
                // Calculate next position
                Vector3 nextZoomOutPosition = Vector3.Lerp(transform.position, maxCursorPosition, zoomOutSpeed);
                float nextZPosition = nextZoomOutPosition.z;

                if (ViewController.ViewType == ViewType.System)
                {
                    if (nextZPosition > ViewController.lowBarrierSystem) // if next position's subViewType is High
                    {
                        lockedUnit = null;
                    }
                }
            }
        }
    }
    private void ResetLockedUnit()
    {
        lockedUnit = null;
    }

    // Utility
    private void OnAwake()
    {
        Instance = this;
        Cam = gameObject.GetComponent<Camera>();

        if (AfterCamera == null)
        {
            AfterCamera = new UnityEvent();
        }
    }
    private void OnStart()
    {
        SetAnchorPosition();

        localPosition = Anchor.transform.InverseTransformPoint(transform.position);
        localTargetPosition = localPosition;

        ViewController.ChangeView.AddListener(ResetLockedUnit);
    }
    public void SetCameraPosition(Vector3 position)
    {
        Anchor.transform.position = Vector3.zero;

        localTargetPosition = Anchor.transform.InverseTransformPoint(position);
        localDragTarget = Anchor.transform.InverseTransformPoint(position);
        worldZoomTarget = position;

        localPosition = Anchor.transform.InverseTransformPoint(position);
        transform.position = position;
    }
    private Vector3 GetScreenLeftWorldPosition()
    {
        Vector3 screenLeftPixelPosition = new Vector3(0, Screen.height / 2f, transform.position.z);

        Vector3 screenLeftWorldPosition = Cam.ScreenToWorldPoint(screenLeftPixelPosition);

        return screenLeftWorldPosition;
    }
    private Vector3 GetScreenRightWorldPosition()
    {
        Vector3 screenRightPixelPosition = new Vector3(Screen.width, Screen.height / 2f, transform.position.z);

        Vector3 screenRightWorldPosition = Cam.ScreenToWorldPoint(screenRightPixelPosition);

        return screenRightWorldPosition;
    }
}