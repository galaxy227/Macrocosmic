using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;

// PopUp for Load Canvas, MenuLoad.cs

public class PopUpDelete : MonoBehaviour
{
    public TextMeshProUGUI Description;

    private void OnEnable()
    {
        string folderName = Path.GetFileName(MenuLoad.Instance.SelectedSaveFolder.saveFolderPath);
        Description.text = "Are you sure you want to delete \n\"" + folderName + "?\"";
    }

    // Buttons
    public void DeleteSelectedFolder()
    {
        Directory.Delete(MenuLoad.Instance.SelectedSaveFolder.saveFolderPath, true);

        UIManager.Instance.BackToLastActiveCanvas();
    }
}
