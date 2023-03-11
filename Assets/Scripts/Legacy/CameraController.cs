using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    public Camera cam;

    [SerializeField] private float lerpTime; // default 25

    public float zoomBarrier; // add/subtract from minZoom and maxZoom in HandleMouseZoom() to prevent X & Z from changing when Y reaches its min/max values
    private float minZoom;
    public float maxZoom;
    public float maxPan;

    // for keys, zoom & drag
    [Range(0f, 1f)] public float movementFactor;  // for keys, how user controls zoom & pan speed, 0-1
    private float movementSpeed;
    private float movementAmount;
    private float movementAmountAsPercentage;
    // for mouse, zoom
    [Range(0f, 1f)] public float zoomFactor; // for mouse, how user controls zoom speed, 0-1
    private float zoomSpeed;
    private float zoomAmount; // physical world distance camera will move when zooming IN (used to calculate how far to move when zooming out)
    private float zoomAmountAsPercentage; // zoomAmount as a percentage of maxZoom (used to LERP camera to minCursorPosition)

    public float zoomPercentage; // interpolation of camera's y value between minimum and maximum zoom

    public Vector3 currentPosition;
    public Vector3 targetPosition; // used to lerp currentPosition to new position
    public Vector3 lockTargetPosition;

    // for Unit Lock
    public static Unit lockedUnit;
    private Vector3 lockOffset;
    private Vector3 localLockPosition;
    private Vector3 worldLockPosition;
    private int framesLocked;
    private int framesUnlocked;

    // for IsPlayerMovingCamera()
    //private Vector3 playerPosition;
    //private Vector3 lastPlayerPosition;
    public static bool isPlayerMovingCamera;

    // mouse zoom
    public Vector3 minCursorPosition;
    public Vector3 maxCursorPosition;
    // mouse drag
    private Vector3 dragStartPosition;
    private Vector3 dragCurrentPosition;
    private bool isMouseOverUIAtDragStartPosition;

    // public static reference for other classes
    public static CameraController CameraControllerInstance;
    public static Camera UnityCam;

    private void Start()
    {
        OnStart();
    }

    private void Update()
    {
        UpdateValues();
        HandleInput();
        SetCoordinateBoundaries();

        SetLockedUnit();
        UpdateLockPosition();

        cam.transform.localPosition = Vector3.Lerp(currentPosition, targetPosition, Time.deltaTime * lerpTime);

        ApplyZoomBarrierBoundary();

        //Debug.Log(isPlayerMovingCamera);

        //Debug.DrawLine(currentPosition, minCursorPosition, Color.red, 1f);
        //Debug.DrawLine(currentPosition, maxCursorPosition, Color.blue, 1f);
    }

    // Input
    private void HandleInput()
    {
        if (!Tools.IsMouseOverUI())
        {
            HandleMouseZoom();
        }

        HandleMouseDrag();
        HandleKeyZoom();
        HandleKeyDrag();

        if (!isPlayerMovingCamera)
        {
            framesLocked++;
            framesUnlocked = 0;
        }
        else
        {
            framesLocked = 0;
            framesUnlocked++;
        }
    }
    // Mouse Input
    private void HandleMouseZoom()
    {
        int mouseScroll = (int)Input.mouseScrollDelta.y;
        int zoomIn = 1;
        int zoomOut = -1;

        if (mouseScroll == zoomIn)
        {
            isPlayerMovingCamera = true;

            if (currentPosition.z > minZoom + zoomBarrier)
            {
                targetPosition = Vector3.Lerp(currentPosition, minCursorPosition, zoomAmountAsPercentage);
            }
        }
        else if (mouseScroll == zoomOut)
        {
            isPlayerMovingCamera = true;

            if (currentPosition.z < maxZoom - zoomBarrier)
            {
                // calculate newZoomAmountAsPercentage to match zoomOut speed with zoomIn speed
                float distance = Vector3.Distance(currentPosition, maxCursorPosition);
                float newZoomAmountAsPercentage = (zoomAmount / distance) * 2f; // multiply by number to make zoom out faster than zoom in

                targetPosition = Vector3.Lerp(currentPosition, maxCursorPosition, newZoomAmountAsPercentage);
            }
        }
    }
    private void HandleMouseDrag()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Plane plane = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragStartPosition = ray.GetPoint(entry);
            }

            isMouseOverUIAtDragStartPosition = Tools.IsMouseOverUI();
        }

        if (Input.GetMouseButton(0))
        {
            Plane plane = new Plane(Vector3.forward, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            float entry;
            if (plane.Raycast(ray, out entry))
            {
                dragCurrentPosition = ray.GetPoint(entry);
            }
        }

        if (!isMouseOverUIAtDragStartPosition)
        {
            if (Input.GetMouseButton(0))
            {
                isPlayerMovingCamera = true;
                targetPosition = cam.transform.localPosition + dragStartPosition - dragCurrentPosition;
            }
        }
    }
    // Keys Input
    private void HandleKeyZoom()
    {
        if (!InputFieldController.IsInputFieldSelected)
        {
            if (Input.GetKey(KeyCode.R))
            {
                isPlayerMovingCamera = true;
                targetPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z + movementAmount);
            }
            if (Input.GetKey(KeyCode.F))
            {
                isPlayerMovingCamera = true;
                targetPosition = new Vector3(currentPosition.x, currentPosition.y, currentPosition.z - movementAmount);
            }
        }
    }
    private void HandleKeyDrag()
    {
        if (!InputFieldController.IsInputFieldSelected)
        {
            float multiplier = movementAmount / 25;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            {
                isPlayerMovingCamera = true;
                targetPosition += transform.up * multiplier;
            }
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            {
                isPlayerMovingCamera = true;
                targetPosition += transform.up * -multiplier;
            }
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            {
                isPlayerMovingCamera = true;
                targetPosition += transform.right * multiplier;
            }
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            {
                isPlayerMovingCamera = true;
                targetPosition += transform.right * -multiplier;
            }
        }
    }

    // Values
    private void UpdateValues()
    {
        currentPosition = cam.transform.localPosition;
        isPlayerMovingCamera = false;

        // zoom, mouse
        SetZoomPercentage();
        SetZoomAmount();
        SetCursorWorldPosition();
        // zoom & drag, keys
        SetMovementAmount();
    }
    private void SetZoomPercentage()
    {
        zoomPercentage = currentPosition.z / maxZoom;
        zoomPercentage = Mathf.Clamp(zoomPercentage, 0f, 1f);
    }
    // Movement, Keys
    private void SetMovementAmount()
    {
        movementSpeed = Mathf.Lerp(maxZoom * 0.05f, maxZoom * 0.25f, movementFactor);
        movementAmountAsPercentage = movementSpeed / maxZoom;
        movementAmount = Vector3.Distance(currentPosition, minCursorPosition) * movementAmountAsPercentage;
    }
    // Zoom, Mouse
    private void SetZoomAmount()
    {
        zoomSpeed = Mathf.Lerp(maxZoom * 0.1f, maxZoom * 0.75f, zoomFactor);
        zoomAmountAsPercentage = zoomSpeed / maxZoom;
        zoomAmount = Vector3.Distance(currentPosition, minCursorPosition) * zoomAmountAsPercentage;
    }
    // Cursor
    private void SetCursorWorldPosition()
    {
        minCursorPosition = GetMinCursorWorldPosition();
        maxCursorPosition = GetMaxCursorWorldPosition();
    }
    private Vector3 GetMinCursorWorldPosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = cam.transform.localPosition.z;

        Vector3 worldPosition = cam.ScreenToWorldPoint(mousePosition);

        return worldPosition;
    }
    private Vector3 GetMaxCursorWorldPosition()
    {
        Vector3 direction = Vector3.zero;
        Vector3 position = currentPosition;

        direction = (currentPosition - minCursorPosition).normalized;

        while (position.z < maxZoom)
        {
            position = position + direction;
        }

        return position;
    }

    // Restrictions
    private void SetCoordinateBoundaries()
    {
        currentPosition.x = Mathf.Clamp(currentPosition.x, -maxPan, maxPan);
        currentPosition.y = Mathf.Clamp(currentPosition.y, -maxPan * 0.55f, maxPan * 0.55f);
        currentPosition.z = Mathf.Clamp(currentPosition.z, minZoom, maxZoom);

        targetPosition.x = Mathf.Clamp(targetPosition.x, -maxPan, maxPan);
        targetPosition.y = Mathf.Clamp(targetPosition.y, -maxPan * 0.55f, maxPan * 0.55f);
        targetPosition.z = Mathf.Clamp(targetPosition.z, minZoom, maxZoom);
    }
    private void ApplyZoomBarrierBoundary()
    {
        if (cam.transform.localPosition.z < zoomBarrier)
        {
            currentPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, zoomBarrier);
            targetPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, zoomBarrier);
            cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, zoomBarrier);
        }
    }

    // Lock
    private void SetLockedUnit()
    {
        if (Input.mouseScrollDelta.y == 1 && !Tools.IsMouseOverUI())
        {
            if (ViewController.ViewType == ViewType.System)
            {
                lockedUnit = InputManager.ClosestUnit;
            }
        }
    }
    private void UpdateLockPosition()
    {
        //------------------------------------------------------------------------------------------------------- LEGACY

        //if (!isPlayerMovingCamera)
        //{
        //    if (lockedUnit != null)
        //    {
        //        lockPosition = lockedUnit.transform.position + lockOffset;

        //        targetPosition = lockPosition;
        //    }
        //}
        //else
        //{
        //    SetLockOffset();
        //}

        //------------------------------------------------------------------------------------------------------- CORRECT, INACCURATE WORLDLOCKPOSITION PAST FRAME 1

        if (!isPlayerMovingCamera)
        {
            if (framesLocked == 1)
            {
                SetLockOffset();
            }

            if (lockedUnit != null)
            {
                targetPosition = lockedUnit.transform.position + lockOffset;
            }
        }
        else
        {
            if (lockedUnit != null)
            {
                if (framesUnlocked == 1)
                {
                    worldLockPosition = currentPosition;
                    localLockPosition = lockedUnit.transform.InverseTransformPoint(currentPosition);
                }

                Vector3 offset = targetPosition - worldLockPosition;

                targetPosition = lockedUnit.transform.TransformPoint(localLockPosition) + offset;
            }
        }

        //------------------------------------------------------------------------------------------------------- INCORRECT, ACCURATE WORLDLOCKPOSITION PAST FRAME 1

        //if (!isPlayerMovingCamera)
        //{
        //    if (lockedUnit != null)
        //    {
        //        if (framesLocked == 1)
        //        {
        //            lockOffset = targetPosition - lockedUnit.transform.position;
        //        }

        //        targetPosition = lockedUnit.transform.position + lockOffset;
        //    }
        //}
        //else
        //{
        //    if (lockedUnit != null)
        //    {
        //        if (framesUnlocked == 1)
        //        {
        //            localLockPosition = lockedUnit.transform.InverseTransformPoint(currentPosition);
        //        }

        //        worldLockPosition = lockedUnit.transform.TransformPoint(localLockPosition);

        //        lockOffset = targetPosition - worldLockPosition;

        //        targetPosition = worldLockPosition + lockOffset;
        //    }
        //}

        //-------------------------------------------------------------------------------------------------------
    }
    private void SetLockOffset()
    {
        if (lockedUnit != null)
        {
            lockOffset = targetPosition - lockedUnit.transform.position;
        }
    }
    private void ResetLockedUnit()
    {
        lockedUnit = null;
    }

    // Utility
    private void OnStart()
    {
        minZoom = 0;

        cam.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, maxZoom);

        // prevent targetPosition from being null (only at start)
        currentPosition = cam.transform.localPosition;
        targetPosition = currentPosition;

        CameraControllerInstance = this;
        UnityCam = cam;

        ViewController.ChangeView.AddListener(ResetLockedUnit);
    }
    //private void IsPlayerMovingCamera()
    //{
    //    playerPosition = currentPosition;

    //    if (playerPosition == lastPlayerPosition)
    //    {
    //        isPlayerMovingCamera = false;
    //    }
    //    else
    //    {
    //        isPlayerMovingCamera = true;
    //    }

    //    lastPlayerPosition = playerPosition;
    //}
}