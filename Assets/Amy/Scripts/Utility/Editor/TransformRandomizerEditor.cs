using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//////////////////////////////////////
//     ©2024 Jennifer Haden         //
//////////////////////////////////////

[CustomEditor(typeof(TransformRandomizer))]
public class TransformRandomizerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        TransformRandomizer mTarget = (TransformRandomizer)target;

        DrawDefaultInspector();

        if (GUILayout.Button("Randomize Transforms"))
        {
            mTarget.randomizeTransforms();
        }
    }
}
