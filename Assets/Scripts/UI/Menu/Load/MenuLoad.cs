using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum SaveFileSortType
{
    FileName,
    Date
}

public class MenuLoad : MonoBehaviour
{
    private MenuLoadFileWatcher FileWatcher;

    // Blocker
    public GameObject LoadSelectedBlocker;
    private ToolTipTrigger toolTipBlocker;
    private Image imageBlocker;
    private Color defaultColorBlocker;
    private Color incompatibleColorBlocker = new Color(150 / 255f, 75 / 255f, 75 / 255f, 25 / 255f);

    // SaveFolder
    public GameObject SaveFolderContent; // Parent
    public SaveFolder SaveFolderPrefab;
    [HideInInspector] public List<SaveFolder> SaveFolderList = new List<SaveFolder>();
    [HideInInspector] public SaveFolder SelectedSaveFolder
    {
        get { return selectedSaveFolder; }
        set
        {
            selectedSaveFolder = value;
            InitializeSaveFileUI();
        }
    }
    private SaveFolder selectedSaveFolder;
    private string lastSelectedSaveFolderName;
    private SaveFileSortType saveFileSortType;
    private bool isAscending;

    // SaveFile 
    public GameObject SaveFileContent; // Parent
    public SaveFile SaveFilePrefab;
    [HideInInspector] public List<SaveFile> SaveFileList = new List<SaveFile>();
    [HideInInspector] public SaveFile SelectedSaveFile
    {
        get { return selectedSaveFile; }
        set 
        { 
            selectedSaveFile = value; 
            SetLoadSelectedBlocker();
        }
    }
    private SaveFile selectedSaveFile;

    // TintSeparator
    public static Color SelectedColor = new Color(240 / 255f, 220 / 255f, 200 / 255f, 7.5f / 255f);
    public static Color DefaultColor = new Color(15 / 255f, 15 / 255f, 15 / 255f, 75 / 255f);
    public static Color NullColor = new Color(15 / 255f, 15 / 255f, 15 / 255f, 0 / 255f);

    public static MenuLoad Instance
    {
        get { return instance; }
    }
    private static MenuLoad instance;

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

        FileWatcher = new MenuLoadFileWatcher();

        // Blocker
        toolTipBlocker = LoadSelectedBlocker.GetComponent<ToolTipTrigger>();
        imageBlocker = LoadSelectedBlocker.GetComponent<Image>();
        defaultColorBlocker = imageBlocker.color;
    }
    private void OnEnable()
    {
        InitializeSaveFolderUI();
    }
    private void Update()
    {
        if (FileWatcher.IsChanged)
        {
            if (!Directory.Exists(SelectedSaveFolder.saveFolderPath))
            {
                SelectedSaveFolder = null;
            }

            InitializeSaveFolderUI();

            FileWatcher.IsChanged = false;
        }
    }

    // Buttons
    public void LoadSelectedGalaxy()
    {
        string folderName = Path.GetFileName(SelectedSaveFolder.saveFolderPath);

        SelectedSaveFile.FileGalaxyDeserialized.LoadGalaxy(folderName);
    }
    public void SetSaveFileSortType(SaveFileSortTypeComponent sortType)
    {
        if (sortType.IsAscending)
        {
            sortType.IsAscending = false;
        }
        else
        {
            sortType.IsAscending = true;                             
        }

        isAscending = sortType.IsAscending;
        saveFileSortType = sortType.SaveFileSortType;

        InitializeSaveFileUI();

        EventSystem.current.SetSelectedGameObject(null);
    }

    // Initialize UI
    private void InitializeSaveFolderUI()
    {
        // Save last selected SaveFolder
        lastSelectedSaveFolderName = null;

        if (SelectedSaveFolder != null)
        {
            lastSelectedSaveFolderName = Path.GetFileName(SelectedSaveFolder.saveFolderPath);
        }

        // Delete previous SaveFolderPrefab UI
        foreach (GameObject gameObj in Tools.GetAllChildren(SaveFolderContent, false))
        {
            Destroy(gameObj);
        }

        SaveFolderList.Clear();

        // Get List of folders in Directory
        List<string> saveFolderPathList = FileHelper.GetListOfFolderPaths(FileGalaxy.MotherSaveFolderPath, true);

        // Instantiate SaveFolderPrefab UI
        for (int i = 0; i < saveFolderPathList.Count; i++)
        {
            // Deserialize FileGalaxy Object
            FileGalaxy fileGalaxy = FileGalaxy.ReadGalaxy(Path.GetFileName(saveFolderPathList[i]), Path.GetFileName(Directory.GetFiles(saveFolderPathList[i])[0]));

            // Instantiate
            SaveFolder saveFolderPrefab = Instantiate(SaveFolderPrefab);
            saveFolderPrefab.transform.SetParent(SaveFolderContent.transform);

            // Set TintSeparator
            if (i % 2 != 0)
            {
                saveFolderPrefab.TintSeparator.color = NullColor;
                saveFolderPrefab.IsTintSeparator = false;
            }
            else
            {
                saveFolderPrefab.TintSeparator.color = DefaultColor;
                saveFolderPrefab.IsTintSeparator = true;
            }

            // Set Text
            saveFolderPrefab.NameText.text = Path.GetFileName(saveFolderPathList[i]);
            saveFolderPrefab.DescriptionText.text = ((GalaxyGenerator.SizeType)fileGalaxy.SettingValues[0]).ToString() + " " + ((GalaxyGenerator.ShapeType)fileGalaxy.SettingValues[1]).ToString() + " Galaxy";

            // Set Path
            saveFolderPrefab.saveFolderPath = saveFolderPathList[i];

            // Add to SaveFolderList
            SaveFolderList.Add(saveFolderPrefab);
        }

        // Set SelectedSaveFolder
        if (saveFolderPathList.Count > 0)
        {
            // Set SelectedSaveFolder as lastSelectedSaveFolderName
            if (lastSelectedSaveFolderName != null)
            {
                foreach (SaveFolder saveFolder in SaveFolderList)
                {
                    if (lastSelectedSaveFolderName == Path.GetFileName(saveFolder.saveFolderPath))
                    {
                        saveFolder.SelectFolder();
                    }
                }
            }
            else
            {
                SaveFolderList[0].SelectFolder();
            }
        }
        else
        {
            SelectedSaveFolder = null;
        }
    }
    private void InitializeSaveFileUI()
    {
        // Delete previous SaveFilePrefab UI
        foreach (GameObject gameObj in Tools.GetAllChildren(SaveFileContent, false))
        {
            Destroy(gameObj);
        }

        SaveFileList.Clear();

        if (SelectedSaveFolder != null)
        {
            // Get List of files in Directory
            string[] saveFilePathArray = Directory.GetFiles(SelectedSaveFolder.saveFolderPath);
            List<string> saveFilePathList = saveFilePathArray.ToList();

            // Instantiate SaveFilePrefab UI
            for (int i = 0; i < saveFilePathList.Count; i++)
            {
                // Instantiate
                SaveFile saveFilePrefab = Instantiate(SaveFilePrefab);

                // Set FileGalaxy
                saveFilePrefab.FileGalaxyDeserialized = FileGalaxy.ReadGalaxy(Path.GetFileName(SelectedSaveFolder.saveFolderPath), Path.GetFileName(saveFilePathList[i]));

                // Set Text
                saveFilePrefab.Name.text = Path.GetFileNameWithoutExtension(saveFilePathList[i]);
                saveFilePrefab.Date.text = saveFilePrefab.FileGalaxyDeserialized.Days.ToString() + "." + saveFilePrefab.FileGalaxyDeserialized.Years.ToString();

                // Set Path
                saveFilePrefab.saveFilePath = saveFilePathList[i];

                // Add to SaveFileList
                SaveFileList.Add(saveFilePrefab);
            }

            // Sort by
            if (saveFileSortType == SaveFileSortType.Date)
            {
                // Sort by Date
                SaveFileList = SaveFileList.OrderBy(o => o.FileGalaxyDeserialized.Years).ThenBy(o => o.FileGalaxyDeserialized.Days).ToList();
            }

            if (isAscending)
            {
                SaveFileList.Reverse();
            }

            for (int i = 0; i < SaveFileList.Count; i++)
            {
                SaveFileList[i].transform.SetParent(SaveFileContent.transform);

                // Set TintSeparator
                if (i % 2 != 0)
                {
                    SaveFileList[i].TintSeparator.color = NullColor;
                    SaveFileList[i].IsTintSeparator = false;
                }
                else
                {
                    SaveFileList[i].TintSeparator.color = DefaultColor;
                    SaveFileList[i].IsTintSeparator = true;
                }
            }
        }

        // Set SelectedSaveFolder
        SelectedSaveFile = null;
    }
    private void SetLoadSelectedBlocker()
    {
        // Set default
        toolTipBlocker.content = "No file selected.";
        imageBlocker.color = defaultColorBlocker;

        if (SelectedSaveFile == null)
        {
            // No file selected
            LoadSelectedBlocker.SetActive(true);
        }
        else
        {
            if (GameController.Instance.VersionObj.IsVersionCompatible(SelectedSaveFile.FileGalaxyDeserialized.VersionData))
            {
                // Compatible version
                LoadSelectedBlocker.SetActive(false);
            }
            else
            {
                // Incompatible version
                toolTipBlocker.content = "Incompatible version.";
                imageBlocker.color = incompatibleColorBlocker;
            }
        }
    }

    // Utility
    public SaveFile GetLatestDateSaveFile()
    {
        SaveFile latestDateSaveFile = null;

        int days = 0;
        int years = 0;

        foreach (SaveFile saveFile in SaveFileList)
        {
            if (saveFile.FileGalaxyDeserialized.Years > years)
            {
                latestDateSaveFile = saveFile;

                days = latestDateSaveFile.FileGalaxyDeserialized.Days;
                years = latestDateSaveFile.FileGalaxyDeserialized.Years;
            }
            else if (saveFile.FileGalaxyDeserialized.Years == years)
            {
                if (saveFile.FileGalaxyDeserialized.Days >= days)
                {
                    latestDateSaveFile = saveFile;

                    days = latestDateSaveFile.FileGalaxyDeserialized.Days;
                    years = latestDateSaveFile.FileGalaxyDeserialized.Years;
                }
            }
        }

        return latestDateSaveFile;
    }

    public class MenuLoadFileWatcher
    {
        public FileSystemWatcher Watcher;
        public bool IsChanged;
        private string path = FileGalaxy.MotherSaveFolderPath;

        public MenuLoadFileWatcher()
        {
            // Create a new FileSystemWatcher and set its properties.
            Watcher = new FileSystemWatcher();
            Watcher.Path = path;
            /* Watch for changes in LastAccess and LastWrite times, and 
               the renaming of files or directories. */
            Watcher.NotifyFilter = NotifyFilters.LastAccess | NotifyFilters.LastWrite
               | NotifyFilters.FileName | NotifyFilters.DirectoryName;
            //// Only watch json files.
            //watcher.Filter = "*.json";

            // Add event handlers.
            Watcher.Changed += new FileSystemEventHandler(OnChanged);
            Watcher.Created += new FileSystemEventHandler(OnChanged);
            Watcher.Deleted += new FileSystemEventHandler(OnChanged);

            // Begin watching.
            Watcher.IncludeSubdirectories = true;
            Watcher.EnableRaisingEvents = true;
        }

        // Event handlers
        private void OnChanged(object source, FileSystemEventArgs e)
        {
            //// Specify what is done when a file is changed, created, or deleted.
            //Debug.Log("File: " + e.FullPath + " " + e.ChangeType);

            IsChanged = true;
        }
    }
}
