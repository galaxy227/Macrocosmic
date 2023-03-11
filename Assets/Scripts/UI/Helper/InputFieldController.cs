using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputFieldController : MonoBehaviour
{
    private TMP_InputField inputField;
    private bool isSelected;

    public static bool IsInputFieldSelected;

    private void Start()
    {
        OnStart();
    }

    private void OnDisable()
    {
        if (isSelected)
        {
            OnDeselect();
        }
    }

    private void OnSelect()
    {
        IsInputFieldSelected = true;
        isSelected = true;
    }
    private void OnDeselect()
    {
        IsInputFieldSelected = false;
        isSelected = false;
    }

    // Utility
    private void OnStart()
    {
        inputField = gameObject.GetComponent<TMP_InputField>();

        inputField.onSelect.AddListener(delegate { OnSelect(); });
        inputField.onDeselect.AddListener(delegate { OnDeselect(); });
    }
}
