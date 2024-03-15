using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Tenkoku.Core;

/* Copyright 2022 Jennifer Haden */
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

        //Only show it if story flag hash is true
        public int titleCardStoryFlagHash = -1;

        public bool isHubRoom = false;
        public bool isOutdoors = true;
        
        public bool freezeTime = false;
        public bool humanDisguise = false;
        public bool dontSpawnPlayer = false;

        public int forceHour = -1;
        public int forceMinute = -1;

        public BGMData bgmData;

        public Color currentFogColor;
       // Tenkoku.Core.TenkokuModule sky;

        public List<GameObject> preloadObjects;

        public float shadow_dist = 1024f;


        public bool isNight = false;
        public bool hasDayNightCycle = false;
        public LightingProfile lighting_day;
        public LightingProfile lighting_night;

        public GameObject daySet;
        public GameObject nightSet;


        void Awake()
        {

        }

        // Update is called once per frame
        void Update()
    	{
           // RenderSettings.fogColor = currentFogColor * RenderSettings.ambientSkyColor;// sky.ambColorGradient.Evaluate(sky.calcTime);
            RenderSettings.fogColor = currentFogColor * RenderSettings.ambientIntensity;// sky.ambColorGradient.Evaluate(sky.calcTime);

            //RenderSettings.fogColor = currentFogColor;

            if (hasDayNightCycle)
            {
                if (PlayerManager.Instance.isNightTime)
                {
                    lighting_night.ApplyToScene();
                }
                else
                {
                    lighting_day.ApplyToScene();
                }
            }
        }


	}

}
