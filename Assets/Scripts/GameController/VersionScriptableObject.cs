using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VersionObj", menuName = "ScriptableObject/VersionObj")]
public class VersionScriptableObject : ScriptableObject
{
    // Version
    [Header("Version Data")]
    public int Major;
    public int Minor;
    public int Patch;
    [Header("Last Compatible Version Data")]
    public int LastMajor;
    public int LastMinor;
    public int LastPatch;
    [Header("Build")]
    public int Build; // Incremented each time the game is built. Is not used, only for developer reference

    public VersionData VersionData
    {
        get { return new VersionData(Major, Minor, Patch); }
    }
    public VersionData LastCompatibleVersionData
    {
        get { return new VersionData(LastMajor, LastMinor, LastPatch); }
    }

    public bool IsVersionCompatible(VersionData version)
    {
        // MAJOR
        if (version.Major > LastCompatibleVersionData.Major)
        {
            // Compatible
            return true;
        }
        else if (version.Major == LastCompatibleVersionData.Major)
        {
            // MINOR
            if (version.Minor > LastCompatibleVersionData.Minor)
            {
                // Compatible
                return true;
            }
            else if (version.Minor == LastCompatibleVersionData.Minor)
            {
                // PATCH
                if (version.Patch >= LastCompatibleVersionData.Patch)
                {
                    // Compatible
                    return true;
                }
                else
                {
                    // Incompatible
                    return false;
                }
            }
            else
            {
                // Incompatible
                return false;
            }
        }
        else
        {
            // Incompatible
            return false;
        }
    }
}

public struct VersionData
{
    public int Major;
    public int Minor;
    public int Patch;
    public string VersionString
    {
        get { return Major + "." + Minor + "." + Patch; }
    }

    public VersionData(int major, int minor, int patch)
    {
        Major = major;
        Minor = minor;
        Patch = patch;
    }
}
