using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System;

public static class FileHandler
{
    public static void SaveListToJSON<T>(List<T> toSave, string folderName, string fileName)
    {
        Debug.Log(GetPath(folderName, fileName));

        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(folderName, fileName), content);
    }

    public static void SaveToJSON<T>(T toSave, string folderName, string fileName)
    {
        Debug.Log(GetPath(folderName, fileName));

        string content = JsonUtility.ToJson(toSave);
        WriteFile(GetPath(folderName, fileName), content);
    }

    public static List<T> ReadListFromJSON<T>(string folderName, string fileName)
    {
        string content = ReadFile(GetPath(folderName, fileName));

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

    public static T ReadFromJSON<T>(string folderName, string fileName)
    {
        string content = ReadFile(GetPath(folderName, fileName));

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

    private static string GetPath(string folderName, string fileName)
    {
        if (string.IsNullOrEmpty(folderName))
        {
            folderName = "/";
        }
        else
        {
            folderName = "/" + folderName + "/";
        }

        return Application.persistentDataPath + folderName + fileName;
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
public static class FileHelper
{
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
        string[] saveFolderPathArray = Directory.GetDirectories(path);
        List<string> saveFolderPathList = saveFolderPathArray.ToList();

        if (removeEmptyFolders)
        {
            // Remove empty folders from SaveFolderPathList
            for (int i = saveFolderPathList.Count - 1; i >= 0; i--)
            {
                if (Directory.GetFiles(saveFolderPathList[i]).Length <= 0)
                {
                    saveFolderPathList.Remove(saveFolderPathList[i]);
                }
            }
        }

        return saveFolderPathList;
    }
}
