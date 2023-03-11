using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SaveFile : MonoBehaviour
{
    public Image TintSeparator;
    public TextMeshProUGUI Name;
    public TextMeshProUGUI Date;

    public FileGalaxy FileGalaxyDeserialized;

    public string saveFilePath;
    public bool IsTintSeparator;

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 1, 1);
    }

    // Buttons
    public void SelectFile()
    {
        foreach (SaveFile saveFile in MenuLoad.Instance.SaveFileList)
        {
            if (saveFile.IsTintSeparator)
            {
                saveFile.TintSeparator.color = MenuLoad.DefaultColor;
            }
            else
            {
                saveFile.TintSeparator.color = MenuLoad.NullColor;
            }
        }

        TintSeparator.color = MenuLoad.SelectedColor;
        MenuLoad.Instance.SelectedSaveFile = this;
    }
}
