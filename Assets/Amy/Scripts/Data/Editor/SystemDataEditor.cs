using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
    [CustomEditor(typeof(SystemData))]
    public class SystemDataEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            SystemData mTarget = (SystemData)target;

            DrawDefaultInspector();
            /*
            if (GUILayout.Button("Copy Hitbox Data Amy -> Cream"))
            {
                mTarget.CreamParams.hitBoxes = mTarget.AmyParams.hitBoxes;
            }
            */
        }

    }
}
