using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public static class FileHandler
{
    public static void SaveListToJSON<T>(List<T> toSave, string path, string fileName)
    {
        path = FileManager.ValidatePath(path);

        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(path + fileName, content);

        Debug.Log(path + fileName);
    }

    public static void SaveToJSON<T>(T toSave, string path, string fileName)
    {
        path = FileManager.ValidatePath(path);

        string content = JsonUtility.ToJson(toSave);
        WriteFile(path + fileName, content);

        Debug.Log(path + fileName);
    }

    public static List<T> ReadListFromJSON<T>(string path, string fileName)
    {
        path = FileManager.ValidatePath(path);

        string content = ReadFile(path + fileName);

        if (string.IsNullOrEmpty(content) || content == "()")
        {
            return new List<T>();
        }
        else
        {
            List<T> result = JsonHelper.FromJson<T>(content).ToList();

            return result;
        }
    }

    public static T ReadFromJSON<T>(string path, string fileName)
    {
        path = FileManager.ValidatePath(path);

        string content = ReadFile(path + fileName);

        if (string.IsNullOrEmpty(content) || content == "()")
        {
            return default(T);
        }
        else
        {
            T result = JsonUtility.FromJson<T>(content);

            return result;
        }
    }

    private static void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using (StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }

    private static string ReadFile(string path)
    {
        if (File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }
        else
        {
            return "";
        }
    }
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}

// My class
public static class FileManager
{
    public static string PlanetsFolderPath
    {
        get
        {
            ValidateFolder(Application.persistentDataPath, planetsFolderName);
            return Application.persistentDataPath + "/" + planetsFolderName;
        }
    }
    private static string planetsFolderName = "Planets";

    public static string SavesFolderPath
    {
        get
        {
            ValidateFolder(Application.persistentDataPath, savesFolderName);
            return Application.persistentDataPath + "/" + savesFolderName;
        }
    }
    private static string savesFolderName = "Saves";

    // Validate
    public static void ValidateFolder(string path, string folderName)
    {
        path = ValidatePath(path);

        if (!Directory.Exists(path + folderName))
        {
            Directory.CreateDirectory(path + folderName);
        }
    }
    public static string ValidatePath(string path)
    {
        if (path[path.Length - 1] != '/' || path[path.Length - 1] != '\\')
        {
            path += "/";
        }

        return path;
    }

    // Duplicate
    public static string HandleDuplicateFolderName(string path, string folderName)
    {
        if (IsSaveFolderNameDuplicate(path, folderName))
        {
            int num = 1;

            do
            {
                num++;
                folderName = folderName + " " + num.ToString();

            } while (IsSaveFolderNameDuplicate(path, folderName));
        }

        return folderName;
    }
    public static bool IsSaveFolderNameDuplicate(string path, string folderName)
    {
        List<string> folderNameList = GetListOfFolderPaths(path, false);

        for (int i = 0; i < folderNameList.Count; i++)
        {
            if (folderName == Path.GetFileName(folderNameList[i]))
            {
                return true;
            }
        }

        return false;
    }
    public static string HandleDuplicateFileName(string path, string fileName)
    {
        string name = Path.GetFileNameWithoutExtension(fileName);
        string extension = Path.GetExtension(fileName);

        if (IsSaveFileNameDuplicate(path, fileName))
        {
            int num = 1;

            do
            {
                num++;
                fileName = name + "_" + num.ToString() + extension;

            } while (IsSaveFileNameDuplicate(path, fileName));
        }

        return fileName;
    }
    public static bool IsSaveFileNameDuplicate(string path, string fileName)
    {
        // Get List of files in Directory
        List<string> fileList = Directory.GetFiles(path).ToList();

        for (int i = 0; i < fileList.Count; i++)
        {
            if (fileName == Path.GetFileName(fileList[i]))
            {
                return true;
            }
        }

        return false;
    }

    // Tools
    public static List<string> GetAllJsonFileNames(string path)
    {
        string[] filePaths = Directory.GetFiles(path, "*.json");

        List<string> fileNames = new List<string>();

        for (int i = 0; i < filePaths.Length; i++)
        {
            fileNames.Add(Path.GetFileName(filePaths[i]));
        }

        return fileNames;
    }
    public static List<string> GetListOfFolderPaths(string path, bool removeEmptyFolders)
    {
        // Get List of folders in Directory
        string[] folderPathArray = Directory.GetDirectories(path);
        List<string> folderPathList = folderPathArray.ToList();

        // Remove empty folders from SaveFolderPathList
        if (removeEmptyFolders)
        {
            for (int i = folderPathList.Count - 1; i >= 0; i--)
            {
                if (Directory.GetFiles(folderPathList[i]).Length <= 0)
                {
                    folderPathList.Remove(folderPathList[i]);
                }
            }
        }

        return folderPathList;
    }
}
