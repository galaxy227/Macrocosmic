using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TaskBar : MonoBehaviour
{
    // Taskbar
    public GameObject TaskBarPanelOutline;
    public GameObject TaskBarPanelFill; // Buttons
    public GameObject TaskBarFolderText; // Text for buttons
    public GameObject TaskBarPanelButtonHighlight; // Selected highlight for button
    // Mask 
    public GameObject TaskBarPanelMask; // Hides "Sub" Panels behind TaskBar
    // "Sub" Panels
    public GameObject PanelData;

    private GameObject activeSubPanel;
    private float moveAmount = 125;
    private float correctAnchor;
    private bool isTaskBarOpen;

    private void OnEnable()
    {
        InitializeTaskBar();
    }

    // Event Triggers
    public void OnPointerEnterButton()
    {
        if (!isTaskBarOpen)
        {
            if (!Input.GetMouseButton(0))
            {
                // TaskBar
                MoveRectTransformAlongXAxis(TaskBarPanelOutline, moveAmount);
                MoveRectTransformAlongXAxis(TaskBarPanelFill, moveAmount);
                MoveRectTransformAlongXAxis(TaskBarFolderText, moveAmount + 75);

                MoveMaskToHideSubPanel(activeSubPanel, moveAmount);

                isTaskBarOpen = true;
            }
        }
    }
    public void OnPointerExitButton()
    {
        if (isTaskBarOpen)
        {
            // TaskBar
            MoveRectTransformAlongXAxis(TaskBarPanelOutline, -moveAmount);
            MoveRectTransformAlongXAxis(TaskBarPanelFill, -moveAmount);
            MoveRectTransformAlongXAxis(TaskBarFolderText, -moveAmount - 75);

            MoveMaskToHideSubPanel(activeSubPanel, -moveAmount);

            isTaskBarOpen = false;
        }
    }
    // Event Triggers Helper
    private void MoveMaskToHideSubPanel(GameObject subPanel, float amount)
    {
        if (subPanel != null)
        {
            subPanel.transform.SetParent(transform, true); // remove child
            MoveRectTransformAlongXAxis(TaskBarPanelMask, amount); // move parent
            subPanel.transform.SetParent(TaskBarPanelMask.transform, true); // re-add child
        }
    }
    private void MoveRectTransformAlongXAxis(GameObject obj, float amount)
    {
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + amount, rect.anchoredPosition.y);
    }

    // Buttons
    public void OpenSubPanel(GameObject panelToOpen)
    {
        HandleOpenSubPanel(panelToOpen);
    }
    // Buttons Helper
    private void HandleOpenSubPanel(GameObject subPanel)
    {
        if (!subPanel.gameObject.activeSelf)
        {
            OnPointerEnterButton(); // Check if isTaskBarOpen true before opening subPanel to fix bug where subPanel disappears when clicking after Alt-Tab, etc

            SetAllSubPanelsFalse();
            subPanel.SetActive(true);

            HandleActiveSubPanel(subPanel);
            SetButtonHighlight(EventSystem.current.currentSelectedGameObject, true);
        }
        else
        {
            subPanel.SetActive(false);
            SetButtonHighlight(EventSystem.current.currentSelectedGameObject, false);
        }

        EventSystem.current.SetSelectedGameObject(null); // Prevent Unity from rendering default button's "highlight" after clicked (even when set to 0 alpha)
    }
    private void HandleActiveSubPanel(GameObject subPanel)
    {
        if (activeSubPanel == null) // Handle Mask With Null Sub Panel
        {
            activeSubPanel = subPanel;

            if (isTaskBarOpen)
            {
                MoveMaskToHideSubPanel(activeSubPanel, moveAmount);
            }

            // Set correctAnchor
            RectTransform rect = subPanel.GetComponent<RectTransform>();
            correctAnchor = rect.anchoredPosition.x;
        }
        else if (activeSubPanel != subPanel) // Handle clicking new subPanel with pre-existing activeSubPanel
        {
            activeSubPanel = subPanel;

            RectTransform rect = subPanel.GetComponent<RectTransform>();

            if (rect.anchoredPosition.x != correctAnchor) // Check for correctAnchor ... switching to another subPanel makes rectTransform.x NOT aligned with TaskBarPanelMask (problem), fix by subtracting moveAmount from X
            {
                rect.anchoredPosition = new Vector2(rect.anchoredPosition.x + -125, rect.anchoredPosition.y);
            }
        }
    }
    private void SetButtonHighlight(GameObject button, bool isActive)
    {
        if (isActive)
        {
            TaskBarPanelButtonHighlight.gameObject.SetActive(true);

            RectTransform buttonRect = button.GetComponent<RectTransform>();
            RectTransform highlightRect = TaskBarPanelButtonHighlight.GetComponent<RectTransform>();

            highlightRect.anchoredPosition = new Vector2(highlightRect.anchoredPosition.x, buttonRect.anchoredPosition.y);
        }
        else
        {
            TaskBarPanelButtonHighlight.gameObject.SetActive(false);
        }
    }
    public void CloseSubPanel(GameObject subPanel)
    {
        subPanel.SetActive(false);
        SetButtonHighlight(EventSystem.current.currentSelectedGameObject, false);
    }

    // Utility
    private void InitializeTaskBar()
    {
        OnPointerExitButton(); // Check if isTaskBarOpen false before opening subPanel to fix bug where TaskBar stays open after clicking Menu, etc

        TaskBarPanelButtonHighlight.gameObject.SetActive(false);
        SetAllSubPanelsFalse();
    } // OnEnable
    private void SetAllSubPanelsFalse()
    {
        PanelData.gameObject.SetActive(false);
    }
}
