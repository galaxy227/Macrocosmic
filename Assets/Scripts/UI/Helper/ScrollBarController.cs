using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScrollBarController : MonoBehaviour
{
    public Color BarColor;
    public Color HandleColor;
    public bool IsHiddenOnExit;

    public Image HorizontalBar;
    public Image HorizontalHandle;
    public Image VerticalBar;
    public Image VerticalHandle;

    private Color HiddenColor = new Color(0, 0, 0, 0);

    private void Awake()
    {
        if (IsHiddenOnExit)
        {
            HideScroll();
        }
        else
        {
            RevealScroll();
        }
    }

    // Event Trigger, Scroll
    public void OnPointerEnterScroll()
    {
        RevealScroll();
    }
    public void OnPointerExitScroll()
    {
        if (IsHiddenOnExit)
        {
            HideScroll();
        }
    }

    private void RevealScroll()
    {
        HorizontalBar.color = BarColor;
        HorizontalHandle.color = HandleColor;
        VerticalBar.color = BarColor;
        VerticalHandle.color = HandleColor;
    }
    private void HideScroll()
    {
        HorizontalBar.color = HiddenColor;
        HorizontalHandle.color = HiddenColor;
        VerticalBar.color = HiddenColor;
        VerticalHandle.color = HiddenColor;
    }
}
