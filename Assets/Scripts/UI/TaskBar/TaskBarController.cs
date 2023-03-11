using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TaskBarController : MonoBehaviour
{
    public GameObject ButtonHighlight;

    public static TaskBarController Instance
    {
        get { return instance; }
    }
    private static TaskBarController instance;

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
        ButtonHighlight.SetActive(false);

        GalaxyGenerator.AfterGenerate.AddListener(OnGenerate);
    }

    public void SetTaskBarView(Canvas canvas, bool isCanvasOpening)
    {
        SetView(isCanvasOpening);
        SetBackground(isCanvasOpening, canvas);

        EventSystem.current.SetSelectedGameObject(null);
    }
    private void SetView(bool isCanvasOpening)
    {
        // Highlight
        SetButtonHighlight(EventSystem.current.currentSelectedGameObject, isCanvasOpening);

        // Canvas
        UIManager.Instance.SetAllStaticCanvas(!isCanvasOpening);

        // Blocker
        UIManager.Instance.BlockerCanvas.gameObject.SetActive(isCanvasOpening);
    }
    private void SetBackground(bool isCanvasOpening, Canvas canvas = null)
    {
        if (isCanvasOpening)
        {
            if (canvas == UIManager.Instance.MiniMenuCanvas)
            {
                BackgroundController.Instance.SetBackground(true, BackgroundType.Gradient);
            }
            else
            {
                UIManager.Instance.TaskBarCanvas.gameObject.SetActive(true);
                BackgroundController.Instance.SetBackground(true, BackgroundType.Fractal);
            }
        }
        else
        {
            BackgroundController.Instance.DisableAllBackgrounds();
        }
    }
    private void SetButtonHighlight(GameObject button, bool isActive)
    {
        if (isActive)
        {
            // Set Position
            if (button != null)
            {
                RectTransform buttonRect = button.GetComponent<RectTransform>();
                RectTransform highlightRect = ButtonHighlight.GetComponent<RectTransform>();
                highlightRect.anchoredPosition = new Vector2(highlightRect.anchoredPosition.x, buttonRect.anchoredPosition.y);

                ButtonHighlight.gameObject.SetActive(true);
            }
        }
        else
        {
            ButtonHighlight.gameObject.SetActive(false);
        }
    }

    // Utility
    private void OnGenerate()
    {
        UIManager.Instance.ToggleActiveCanvas(UIManager.Instance.CurrentActiveCanvas);
    }
}
