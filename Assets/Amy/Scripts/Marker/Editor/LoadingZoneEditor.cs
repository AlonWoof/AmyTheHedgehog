using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[CustomEditor(typeof(LoadingZone))]
	public class LoadingZoneEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            LoadingZone door = (LoadingZone)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Load Target Scene"))
            {
                EditorSceneManager.SaveOpenScenes();

                string scnpath = "";

                string[] paths = AssetDatabase.FindAssets("t: Scene " + door.targetScene, null);

                foreach (string guid in paths)
                {
                    scnpath = AssetDatabase.GUIDToAssetPath(guid);
                    Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                }

                EditorSceneManager.OpenScene(scnpath);
            }

        }
    }
}
