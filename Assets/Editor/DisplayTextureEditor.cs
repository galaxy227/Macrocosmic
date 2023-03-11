using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DisplayTexture))]
public class DisplayTextureEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DisplayTexture display = (DisplayTexture)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate"))
        {
            display.DrawTexture();
        }
    }
}
