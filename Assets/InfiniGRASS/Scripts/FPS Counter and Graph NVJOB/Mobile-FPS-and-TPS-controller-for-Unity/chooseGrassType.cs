using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Artngame.INfiniDy;

public class chooseGrassType : MonoBehaviour
{
    public int grassType = 0;
    public InfiniGRASSManager grassManager;
    public Vector2 screenResolution = new Vector2(1480,720);
    // Start is called before the first frame update
    void Start()
    {
        Screen.orientation = ScreenOrientation.LandscapeLeft;
       // Screen.SetResolution((int)screenResolution.x, (int)screenResolution.y, true);
    }
    void LoadBrush(InfiniGRASS_BrushSettings Setting1)
    {

        //v1.2a
        grassManager.min_scale = Setting1.min_scale;
        grassManager.max_scale = Setting1.max_scale;
        grassManager.Min_density = Setting1.Min_density;
        grassManager.Max_density = Setting1.Max_density;
        grassManager.Min_spread = Setting1.Min_spread;
        grassManager.Max_spread = Setting1.Max_spread;

        //Debug.Log("Min_spread = " + grassManager.Min_spread);
        //Debug.Log("Max_spread = " + grassManager.Max_spread);

        grassManager.SpecularPower = Setting1.SpecularPower;
        grassManager.Cutoff_distance = Setting1.Cutoff_distance;
        grassManager.LOD_distance = Setting1.LOD_distance;
        grassManager.LOD_distance1 = Setting1.LOD_distance1;
        grassManager.LOD_distance2 = Setting1.LOD_distance2;

        grassManager.AmplifyWind = Setting1.AmplifyWind;
        grassManager.WindTurbulence = Setting1.WindTurbulence;

        //grassManager.Editor_view_dist = 4500*(grassManager.WorldScale/20);
        grassManager.min_grass_patch_dist = Setting1.min_grass_patch_dist;
        grassManager.Stop_Motion_distance = Setting1.Stop_Motion_distance;

        //					for(int i=0;i<grassManager.GrassMaterials.Count;i++){
        //						grassManager.GrassMaterials[i].SetFloat("_SmoothMotionFactor",255);
        //						grassManager.GrassMaterials[i].SetVector("_TimeControl1",new Vector4(2,1,1,0));
        //					}
        grassManager.Override_density = Setting1.Override_density;
        grassManager.Override_spread = Setting1.Override_spread;
        grassManager.MinAvoidDist = Setting1.MinAvoidDist;
        grassManager.MinScaleAvoidDist = Setting1.MinScaleAvoidDist;
        grassManager.SphereCastRadius = Setting1.SphereCastRadius;
        grassManager.Grass_Fade_distance = Setting1.Grass_Fade_distance;
        //grassManager.Gizmo_scale = (grassManager.WorldScale/20)*3;
        grassManager.Collider_scale = Setting1.Collider_scale;

        //REST
        grassManager.Min_density = Setting1.Min_density;
        grassManager.Max_density = Setting1.Max_density;
        grassManager.rayCastDist = Setting1.rayCastDist;
        grassManager.InteractionSpeed = Setting1.InteractionSpeed;
        grassManager.InteractSpeedThres = Setting1.InteractSpeedThres;
        grassManager.Interaction_thres = Setting1.Interaction_thres;
        grassManager.Max_tree_dist = Setting1.Max_tree_dist;//v1.4.6
        grassManager.Interaction_offset = Setting1.Interaction_offset;
        grassManager.RandRotMin = Setting1.RandRotMin;
        grassManager.RandRotMax = Setting1.RandRotMax;
        grassManager.RandomRot = Setting1.RandomRot;

        grassManager.GroupByObject = Setting1.GroupByObject;
        grassManager.ParentToObject = Setting1.ParentToObject;
        grassManager.MoveWithObject = Setting1.MoveWithObject;
        grassManager.AvoidOwnColl = Setting1.AvoidOwnColl;

        grassManager.AdaptOnTerrain = Setting1.AdaptOnTerrain;
        grassManager.Max_interactive_group_members = Setting1.Max_interactive_group_members;
        grassManager.Max_static_group_members = Setting1.Max_static_group_members;
        grassManager.Interactive = Setting1.Interactive;
        grassManager.GridOnNormal = Setting1.GridOnNormal;

    }
    public float playerSpeed = 100;
    void OnGUI()
    {
        float h = Screen.height; float wh = Screen.width;
        GUI.skin.horizontalSlider.fixedHeight = h * 3 / 100;
        GUI.skin.horizontalSliderThumb.fixedHeight = h * 3 / 100;
        GUI.skin.horizontalSliderThumb.fixedWidth = wh * 2 / 100;

        if (GUI.Button(new Rect(Screen.width / 2, Screen.height -130, 100, 30), "Move Front"))
        {
            transform.position = transform.position + transform.forward * Time.deltaTime * playerSpeed;
        }
        if (GUI.Button(new Rect(Screen.width / 2, Screen.height - 100, 100, 30), "Move Back"))
        {
            transform.position = transform.position - transform.forward * Time.deltaTime * playerSpeed;
        }
        if (GUI.Button(new Rect(Screen.width / 2 - 100, Screen.height - 130, 100, 30), "Move Left"))
        {
            transform.position = transform.position - transform.right * Time.deltaTime * playerSpeed;
        }
        if (GUI.Button(new Rect(Screen.width / 2 + 100, Screen.height - 130, 100, 30), "Move Right"))
        {
            transform.position = transform.position + transform.right * Time.deltaTime * playerSpeed;
        }

        if (GUI.Button(new Rect(Screen.width/2,10,100,30),"Cycle type")) {
            if(grassType == 0)
            {
                grassType = 17;
                grassManager.Grass_selector = 17;
                InfiniGRASS_BrushSettings Setting1 = new InfiniGRASS_BrushSettings();
                Setting1 = grassManager.BrushSettings[grassManager.Grass_selector];
                LoadBrush(Setting1);
               // Debug.Log("Loaded type 17");
            }
            else
            {
                grassType = 0;
                grassManager.Grass_selector = 0;
                InfiniGRASS_BrushSettings Setting1 = new InfiniGRASS_BrushSettings();
                Setting1 = grassManager.BrushSettings[grassManager.Grass_selector];
                LoadBrush(Setting1);
               // Debug.Log("Loaded type 0");
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (grassType == 17)
        {
            grassType = 17;
            grassManager.Grass_selector = 17;
            InfiniGRASS_BrushSettings Setting1 = new InfiniGRASS_BrushSettings();
            Setting1 = grassManager.BrushSettings[grassManager.Grass_selector];
            LoadBrush(Setting1);
           // Debug.Log("Loaded type 17");
        }
        else
        {
            grassType = 0;
            grassManager.Grass_selector = 0;
            InfiniGRASS_BrushSettings Setting1 = new InfiniGRASS_BrushSettings();
            Setting1 = grassManager.BrushSettings[grassManager.Grass_selector];
            LoadBrush(Setting1);
           // Debug.Log("Loaded type 0");
        }
    }
}
