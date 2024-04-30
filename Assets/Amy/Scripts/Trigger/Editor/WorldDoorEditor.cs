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
	[CustomEditor(typeof(WorldDoor))]
	public class WorldDoorEditor : Editor
	{
        public override void OnInspectorGUI()
        {
            WorldDoor door = (WorldDoor)target;

            DrawDefaultInspector();

            if (GUILayout.Button("Load Target Scene"))
            {
                EditorSceneManager.SaveOpenScenes();

                string scnpath = "";
                
                string[] paths = AssetDatabase.FindAssets("t: Scene " + door.destinationScene, null);

                foreach(string guid in paths)
                {
                    scnpath = AssetDatabase.GUIDToAssetPath(guid);
                    Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
                }

                int exit = door.destinationExit;

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
