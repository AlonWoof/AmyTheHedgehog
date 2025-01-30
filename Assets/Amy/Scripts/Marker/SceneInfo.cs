using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
//using Tenkoku.Core;

/* Copyright 2025 Jennifer Haden */
namespace Amy
{

    [System.Serializable]
    public class LightingProfile
    {
        public Material skybox_mat;

        [ColorUsage(true, true)]
        public Color light_sky;

        [ColorUsage(true, true)]
        public Color light_equator;

        [ColorUsage(true, true)]
        public Color light_ground;

        public Color fog_color;
        public float fog_density;

        public void ApplyToScene()
        {
            RenderSettings.skybox = skybox_mat;
            RenderSettings.fogColor = fog_color;
            RenderSettings.ambientSkyColor = light_sky;
            RenderSettings.ambientEquatorColor = light_equator;
            RenderSettings.ambientGroundColor = light_ground;
            RenderSettings.fogDensity = fog_density;
        }

        public void getFromScene()
        {
            fog_color = RenderSettings.fogColor;
            light_sky = RenderSettings.ambientSkyColor;
            light_equator = RenderSettings.ambientEquatorColor;
            light_ground = RenderSettings.ambientGroundColor;
            fog_density = RenderSettings.fogDensity;
        }
    }

	public class SceneInfo : MonoBehaviour
	{

        public string areaName = "Default Zone";
        public bool showTitleCard = false;

        public Color cameraClearColor = Color.black;

        //Only show it if story flag hash is true
        public int titleCardStoryFlagHash = -1;

        public bool isHubWorld = false;
        public bool isSmallRoom = false;
        public bool isOutdoors = true;
        
        public bool dontSpawnPlayer = false;
        public bool forceYoungAmy = false;


        public BGMData bgmData;

        public List<GameObject> preloadObjects;

        public float shadow_dist = 1024f;

        public PostProcessProfile postProcessProfile;

        void Awake()
        {
            GameManager.Instance.setCameraClearColor(cameraClearColor);
           // Screen.SetResolution(640, 480, true);
        }

        // Update is called once per frame
        void Update()
    	{


        }

        private void OnValidate()
        {
            
            gameObject.name = "SceneInfo";

            if(Camera.main)
                Camera.main.backgroundColor = cameraClearColor;

            gameObject.layer = LayerMask.NameToLayer("PostProcessing");

            if (!GetComponent<PostProcessVolume>())
            {
                PostProcessVolume pp = gameObject.AddComponent<PostProcessVolume>();
                pp.isGlobal = true;
                pp.profile = postProcessProfile;
            }

        }


    }

}
