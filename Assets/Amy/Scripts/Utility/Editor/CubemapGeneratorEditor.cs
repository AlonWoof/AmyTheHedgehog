using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//////////////////////////////////////
//     ©2024 Jennifer Haden         //
//////////////////////////////////////

[CustomEditor(typeof(CubemapGenerator))]
public class CubemapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        CubemapGenerator mTarget = (CubemapGenerator)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Generate Cubemap"))
        {
            mTarget.generateCubemapTextures();
        }
    }

}

