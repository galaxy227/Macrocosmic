using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI VersionText;

    private void Start()
    {
        OnStart();
    }

    // Buttons
    public void QuitApplication()
    {
        Application.Quit();
    }

    // Utility
    private void OnStart()
    {
        SetVersionText();
    }
    private void SetVersionText()
    {
        VersionText.text = "v" + GameController.Instance.Version.VersionString + "." + Tools.GetBuildNumber().BuildNumber;
    }
}
