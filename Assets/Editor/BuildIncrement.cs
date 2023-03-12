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
        BuildNumberScriptableObject previousBuildObj = Tools.GetBuildNumber();

        // New
        BuildNumberScriptableObject newBuildObj = ScriptableObject.CreateInstance<BuildNumberScriptableObject>();
        newBuildObj.BuildNumber = previousBuildObj.BuildNumber + 1;

        // Save
        AssetDatabase.DeleteAsset("Assets/Resources/Build.asset"); // Delete any old build.asset
        AssetDatabase.CreateAsset(newBuildObj, "Assets/Resources/Build.asset"); // Create the new one with correct build number before build starts
        AssetDatabase.SaveAssets();
    }
}
