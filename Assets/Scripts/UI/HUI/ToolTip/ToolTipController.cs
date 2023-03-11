using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolTipController : MonoBehaviour
{
    public ToolTip toolTip;

    private static ToolTipController instance;

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
        Hide();
    }

    public static void Show(string content, string header = "")
    {
        instance.toolTip.SetText(content, header);

        instance.toolTip.gameObject.SetActive(true);
    }

    public static void Hide()
    {
        instance.toolTip.gameObject.SetActive(false);
    }
}
