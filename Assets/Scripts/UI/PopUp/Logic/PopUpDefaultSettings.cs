using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// PopUp for Settings canvas, MenuSettings.cs

public class PopUpDefaultSettings : MonoBehaviour
{
    private void OnEnable()
    {
        MenuSettings.Instance.isPopUpCancel = true;
    }

    // Buttons
    public void SetDefault()
    {
        MenuSettings.Instance.SetDefaultSettings();

        MenuSettings.Instance.isPopUpCancel = false;
        UIManager.Instance.BackToLastActiveCanvas();
    }
}
