using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopUp : MonoBehaviour
{
    public static PopUp Instance
    {
        get { return instance; }
    }
    private static PopUp instance;
    private int enableCount;

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

        enableCount = 0;
    }
    private void OnEnable()
    {
        if (enableCount > 0)
        {
            BackgroundController.Instance.SetBackground(true, BackgroundType.PopUp, DelayType.Fast);
        }

        enableCount++;
    }
    private void OnDisable()
    {
        BackgroundController.Instance.SetBackground(false, BackgroundType.PopUp, DelayType.Fast);
    }
}
