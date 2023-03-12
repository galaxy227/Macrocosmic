using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using Unity.VectorGraphics;

// SaveFolder UI object for MenuLoad.cs

public class SaveFolder : MonoBehaviour
{
    // Blocker
    public Image LoadLatestBlocker;
    public SVGImage IncompatibleVersionSprite;
    private ToolTipTrigger spriteToolTip;

    public Image TintSeparator;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescriptionText;

    public string saveFolderPath;
    public bool IsTintSeparator;

    private void Start()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = new Vector3(1, 1, 1);

        // Blocker
        spriteToolTip = IncompatibleVersionSprite.GetComponent<ToolTipTrigger>();
        SetLoadLatestBlocker();
    }

    // Buttons
    public void SelectFolder()
    {
        if (MenuLoad.Instance.SelectedSaveFolder != this)
        {
            foreach (SaveFolder saveFolder in MenuLoad.Instance.SaveFolderList)
            {
                if (saveFolder.IsTintSeparator)
                {
                    saveFolder.TintSeparator.color = MenuLoad.DefaultColor;
                }
                else
                {
                    saveFolder.TintSeparator.color = MenuLoad.NullColor;
                }
            }

            TintSeparator.color = MenuLoad.SelectedColor;
            MenuLoad.Instance.SelectedSaveFolder = this;
        }
    }
    public void LoadLatestGalaxy()
    {
        SelectFolder();

        SaveFile saveFile = MenuLoad.Instance.GetLatestDateSaveFile();
        string folderName = Path.GetFileName(Path.GetDirectoryName(saveFile.saveFilePath));

        saveFile.FileGalaxyDeserialized.LoadGalaxy(folderName);
    }

    // Blocker
    private void SetLoadLatestBlocker()
    {
        string[] filePathArray = Directory.GetFiles(saveFolderPath);
        FileGalaxy fileGalaxy = FileGalaxy.ReadGalaxy(Path.GetFileName(saveFolderPath), Path.GetFileName(filePathArray[0]));
        
        if (!GameController.Instance.VersionObj.IsVersionCompatible(fileGalaxy.VersionData))
        {
            LoadLatestBlocker.gameObject.SetActive(true);
            IncompatibleVersionSprite.gameObject.SetActive(true);

            spriteToolTip.content = "v" + fileGalaxy.VersionData.VersionString + " is incompatible with " + "v" + GameController.Instance.VersionObj.VersionData.VersionString;
        }
        else
        {
            LoadLatestBlocker.gameObject.SetActive(false);
            IncompatibleVersionSprite.gameObject.SetActive(false);
        }
    }
}
