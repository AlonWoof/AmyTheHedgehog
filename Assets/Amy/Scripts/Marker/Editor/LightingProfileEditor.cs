using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	[CustomEditor(typeof(SceneInfo))]
	public class SceneInfoEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            SceneInfo mInfo = (SceneInfo)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Get Day Lighting from current lighting"))
            {
                mInfo.lighting_day.getFromScene();
            }

            if (GUILayout.Button("Get Night Lighting from current lighting"))
            {
                mInfo.lighting_night.getFromScene();
            }
        }
    }
}
