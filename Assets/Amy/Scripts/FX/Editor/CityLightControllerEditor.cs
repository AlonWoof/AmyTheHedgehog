using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[CustomEditor(typeof(CityLightController))]
	public class CityLightControllerEditor : Editor
	{
        //DEPRECATED
        /*
        public override void OnInspectorGUI()
        {
            CityLightController lc = (CityLightController)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Turn ON all lights"))
            {
                lc.turnOnAllLights();
            }

            if (GUILayout.Button("Turn OFF all lights"))
            {
                lc.turnOffAllLights();
            }

        }
        */
    }
}
