using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

// Saves state of galaxy ...
// Version of game
// Seed
// Date
// Satellite positions

public class FileGalaxy
{
    // PATH
    public static string CurrentSaveFolderPath
    {
        get
        {
            return FileManager.SavesFolderPath + "/" + CurrentSaveFolderName;
        }
    }
    public static string CurrentSaveFolderName;

    // SAVE FILE
    public string FileName;
    // GameController
    public VersionData VersionData
    {
        get 
        {
            return new VersionData(VersionDataArray[0], VersionDataArray[1], VersionDataArray[2]); 
        }
    }
    [SerializeField] private int[] VersionDataArray = new int[3];
    // GalaxyGenerator
    public int Seed;
    public string Name;
    public List<int> SettingValues = new List<int>();
    public List<float> MultiplierValues = new List<float>();
    // TimeController, Date
    public int Days; 
    public int Years;
    // Satellite LocalPositions
    public List<Vector3> LocalPositions = new List<Vector3>(); 

    // Save
    public FileGalaxy(string fileName)
    {
        FileName = fileName;

        // GameController
        VersionDataArray[0] = GameController.Instance.VersionObj.Major;
        VersionDataArray[1] = GameController.Instance.VersionObj.Minor;
        VersionDataArray[2] = GameController.Instance.VersionObj.Patch;

        // GalaxyGenerator
        Seed = GalaxyGenerator.Instance.Seed;
        Name = GalaxyGenerator.Instance.Name;

        SettingValues.Add((int)GalaxyGenerator.Instance.sizeType);
        SettingValues.Add((int)GalaxyGenerator.Instance.shapeType);
        SettingValues.Add(GalaxyGenerator.Instance.NumArms);

        MultiplierValues.Add(SystemGenerator.IceMultiplier);
        MultiplierValues.Add(SystemGenerator.OceanicMultiplier);
        MultiplierValues.Add(SystemGenerator.GaiaMultiplier);
        MultiplierValues.Add(SystemGenerator.TemperateMultiplier);
        MultiplierValues.Add(SystemGenerator.DesertMultiplier);
        MultiplierValues.Add(SystemGenerator.VolcanicMultiplier);
        MultiplierValues.Add(SystemGenerator.ToxicMultiplier);
        MultiplierValues.Add(SystemGenerator.BarrenMultiplier);
        MultiplierValues.Add(SystemGenerator.GasGiantMultiplier);

        // TimeController, Date
        Days = TimeController.Instance.Days;
        Years = TimeController.Instance.Years;

        // Satellite LocalPositions
        for (int i = 0; i < SatelliteController.Instance.CentralBodySatelliteList.Count; i++)
        {
            LocalPositions.Add(SatelliteController.Instance.CentralBodySatelliteList[i].transform.localPosition);
        }
    }

    // Load
    public void LoadGalaxy(string folderName)
    {
        //ValidateParentSaveFolder();

        // Deserialize
        if (GameController.Instance.VersionObj.IsVersionCompatible(VersionData)) // if file version is compatible with LastCompatibleVersion
        {
            // GameState
            if (GameController.Instance.GameState != GameState.Play)
            {
                GameController.Instance.SetGameState(GameState.Play);
            }

            // GalaxyGenerator
            GalaxyGenerator.Instance.inputSeed = Seed;
            GalaxyGenerator.Instance.Seed = Seed;

            GalaxyGenerator.Instance.Name = Name;

            GalaxyGenerator.Instance.sizeType = (SizeType)SettingValues[0];
            GalaxyGenerator.Instance.shapeType = (GalaxyGenerator.ShapeType)SettingValues[1];
            GalaxyGenerator.Instance.NumArms = SettingValues[2];

            SystemGenerator.IceMultiplier = MultiplierValues[0];
            SystemGenerator.OceanicMultiplier = MultiplierValues[1];
            SystemGenerator.GaiaMultiplier = MultiplierValues[2];
            SystemGenerator.TemperateMultiplier = MultiplierValues[3];
            SystemGenerator.DesertMultiplier = MultiplierValues[4];
            SystemGenerator.VolcanicMultiplier = MultiplierValues[5];
            SystemGenerator.ToxicMultiplier = MultiplierValues[6];
            SystemGenerator.BarrenMultiplier = MultiplierValues[7];
            SystemGenerator.GasGiantMultiplier = MultiplierValues[8];

            CurrentSaveFolderName = folderName;
            GalaxyGenerator.Instance.ExecuteGenerate(true); 

            // Date
            TimeController.Instance.SetDate(Days, Years);

            // Satellite LocalPositions
            for (int i = 0; i < LocalPositions.Count; i++)
            {
                SatelliteController.Instance.CentralBodySatelliteList[i].transform.localPosition = LocalPositions[i];
            }
        }
    }

    // Save
    public static void SaveGalaxy(string fileName)
    {
        fileName = fileName + ".json";

        // Create new CurrentSaveFolder for saves for this galaxy
        if (CurrentSaveFolderName == null || !FileManager.IsSaveFolderNameDuplicate(FileManager.SavesFolderPath, CurrentSaveFolderName)) // if Galaxy is new OR if Folder does not exist
        {
            CurrentSaveFolderName = GalaxyGenerator.Instance.Name;

            // Check if CurrentSaveFolder is duplicate to avoid overwriting pre-existing folders
            CurrentSaveFolderName = FileManager.HandleDuplicateFolderName(FileManager.SavesFolderPath, CurrentSaveFolderName);

            // Create Folder
            Directory.CreateDirectory(CurrentSaveFolderPath);
        }

        // Check if fileName is duplicate to avoid overwriting pre-existing files
        fileName = FileManager.HandleDuplicateFileName(CurrentSaveFolderPath, fileName);

        FileGalaxy fileGalaxy = new FileGalaxy(fileName);
        FileHandler.SaveToJSON<FileGalaxy>(fileGalaxy, FileManager.SavesFolderPath + "/" + CurrentSaveFolderName, fileGalaxy.FileName);
    }

    // Read
    public static FileGalaxy ReadGalaxy(string folderName, string fileName)
    {
        return FileHandler.ReadFromJSON<FileGalaxy>(FileManager.SavesFolderPath + "/" + folderName, fileName);
    }
}
