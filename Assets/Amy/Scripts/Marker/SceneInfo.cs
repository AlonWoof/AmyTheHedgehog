using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Tenkoku.Core;

/* Copyright 2022 Jason Haden */
namespace Amy
{

	public class SceneInfo : MonoBehaviour
	{

        public string areaName = "Default Zone";
        public bool showTitleCard = false;

        //Only show it if story flag hash is true
        public int titleCardStoryFlagHash = -1;

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

        void Awake()
        {
            //if (forceHour > -1)
            //    TimeManager.Instance.hour = forceHour;

            //if (forceMinute > -1)
            //    TimeManager.Instance.minute = forceMinute;

            //TimeManager.Instance.timeFrozen = freezeTime;


            //sky = FindObjectOfType<TenkokuModule>();
            currentFogColor = RenderSettings.fogColor;

            QualitySettings.shadowDistance = shadow_dist;
        }

        // Update is called once per frame
        void Update()
    	{
           // RenderSettings.fogColor = currentFogColor * RenderSettings.ambientSkyColor;// sky.ambColorGradient.Evaluate(sky.calcTime);
            RenderSettings.fogColor = currentFogColor * RenderSettings.ambientIntensity;// sky.ambColorGradient.Evaluate(sky.calcTime);

            //RenderSettings.fogColor = currentFogColor;
        }
	}

}
