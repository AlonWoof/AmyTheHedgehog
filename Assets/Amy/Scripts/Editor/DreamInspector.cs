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

    public class WarpInfo
    {
        public GameObject obj;
        public string sceneName;
        public int exitNumber;
    }

    public class DreamInspector : EditorWindow
    {
        public SceneInfo scn_info;
        public string scn_name;
        public int ringCount = 0;
        public int numExits = 0;
        public bool warning_ringCloseToExit = false;

        List<WarpInfo> warps;

        // Add menu item named "My Window" to the Window menu
        [MenuItem("Amy/Dream Inspector")]
        public static void ShowWindow()
        {
            //Show existing window instance. If one doesn't exist, make one.
            EditorWindow.GetWindow(typeof(DreamInspector));

        }

        void OnInspectorUpdate()
        {
            checkAllWarnings();
        }

        void OnGUI()
        {
            GUILayout.Label("Scene Information", EditorStyles.boldLabel);

            

            if(warning_ringCloseToExit)
                GUILayout.Label("WARNING: Ring close to exit. This can result in cheap farming.", EditorStyles.boldLabel);


            if (scn_name != UnityEngine.SceneManagement.SceneManager.GetActiveScene().name)
            {
                scn_name = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
                refreshAllInfo();
            }



            if (scn_info != null)
            {
                if(scn_info.bgmData)
                    GUILayout.Label("Music: " + scn_info.bgmData.songName);
                else
                    GUILayout.Label("Music: None");

            }

            GUILayout.Label("Ring Count: " + ringCount, EditorStyles.label);

            if (warps == null)
                gatherWarpInfo();

            foreach (WarpInfo w in warps)
            {
                GUILayout.Label("Warp: " + w.sceneName + " EXIT: " + w.exitNumber, EditorStyles.label);
                if (GUILayout.Button("Follow Warp"))
                    loadWarp(w);
            }

        }

        public void refreshAllInfo()
        {
            findSceneInfo();
            gatherWarpInfo();
            countRings();
            findSceneInfo();
        }

        public void findSceneInfo()
        {
            if(!scn_info)
                scn_info = FindObjectOfType<SceneInfo>();
        }

        public void countRings()
        {
            ringCount = FindObjectsOfType<Ring>().Length;
        }

        public void gatherWarpInfo()
        {
            if (warps == null)
                warps = new List<WarpInfo>();
            else
                warps.Clear();

            foreach(WorldDoor door in FindObjectsOfType<WorldDoor>())
            {
                WarpInfo w = new WarpInfo();

                w.obj = door.gameObject;
                w.exitNumber = door.destinationExit;
                w.sceneName = door.destinationScene;

                warps.Add(w);
            }

            foreach(LoadingZone zone in FindObjectsOfType<LoadingZone>())
            {
                WarpInfo w = new WarpInfo();

                w.obj = zone.gameObject;
                w.exitNumber = zone.exitNumber;
                w.sceneName = zone.targetScene;

                warps.Add(w);
            }

            foreach(CityElevator e in FindObjectsOfType<CityElevator>())
            {
                WarpInfo w = new WarpInfo();

                w.obj = e.gameObject;
                w.exitNumber = e.sceneExit;
                w.sceneName = e.sceneToLoad;

                warps.Add(w);
            }
        }


        public void loadWarp(WarpInfo warp)
        {

            EditorSceneManager.SaveOpenScenes();
            scn_info = null;

            string scnpath = "";

            string[] paths = AssetDatabase.FindAssets("t: Scene " + warp.sceneName, null);

            foreach (string guid in paths)
            {
                scnpath = AssetDatabase.GUIDToAssetPath(guid);
                Debug.Log(AssetDatabase.GUIDToAssetPath(guid));
            }

            int exit = warp.exitNumber;

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

        public void checkAllWarnings()
        {
            ringCloseToExitWarning();
        }

        public void ringCloseToExitWarning()
        {
            warning_ringCloseToExit = false;

            foreach (Ring r in FindObjectsOfType<Ring>())
            {
                foreach(WarpInfo w in warps)
                {
                    float dst = Vector3.Distance(w.obj.transform.position, r.transform.position);

                    if(dst < 10.0f)
                    {
                        warning_ringCloseToExit = true;
                        
                    }
                }
            }
        }
    }
}
