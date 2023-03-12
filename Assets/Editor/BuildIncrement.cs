using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class BuildIncrement : IPreprocessBuildWithReport
{
    public int callbackOrder => 1;

    public void OnPreprocessBuild(BuildReport report)
    {
        // Previous
        VersionScriptableObject versionObj = Tools.GetVersionObject();

        // New
        VersionScriptableObject newVersionObj = ScriptableObject.CreateInstance<VersionScriptableObject>();

        newVersionObj.Major = versionObj.Major;
        newVersionObj.Minor = versionObj.Minor;
        newVersionObj.Patch = versionObj.Patch;
        newVersionObj.LastMajor = versionObj.LastMajor;
        newVersionObj.LastMinor = versionObj.LastMinor;
        newVersionObj.LastPatch = versionObj.LastPatch;

        newVersionObj.Build = versionObj.Build + 1;

        // Save
        AssetDatabase.DeleteAsset("Assets/Resources/VersionObj.asset"); // Delete any old build.asset
        AssetDatabase.CreateAsset(newVersionObj, "Assets/Resources/VersionObj.asset"); // Create the new one with correct build number before build starts
        AssetDatabase.SaveAssets();
    }
}
