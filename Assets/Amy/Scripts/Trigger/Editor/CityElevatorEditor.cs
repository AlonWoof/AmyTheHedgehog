using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	
    [CustomEditor(typeof(CityElevator))]
    public class CityElevatorEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            CityElevator door = (CityElevator)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Load Target Scene"))
            {
                EditorSceneManager.SaveOpenScenes();

                string scnpath = "";

                string[] paths = AssetDatabase.FindAssets("t: Scene " + door.sceneToLoad, null);

                foreach (string guid in paths)
                {
                    if (AssetDatabase.GUIDToAssetPath(guid).ToLower().Contains(door.sceneToLoad.ToLower() + ".unity"))
                    {
                        scnpath = AssetDatabase.GUIDToAssetPath(guid);
                        Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                    }
                }

                int exit = door.sceneExit;

                EditorSceneManager.OpenScene(scnpath);


                foreach (Exit e in FindObjectsOfType<Exit>())
                {

                    if (e.exitNumber == exit)
                    {
                        SceneView.lastActiveSceneView.pivot = e.transform.position + Vector3.up;
                        SceneView.lastActiveSceneView.LookAt(e.transform.position + Vector3.up + e.transform.forward);
                        SceneView.lastActiveSceneView.Repaint();
                    }
                }
            }

        }
    }
}
