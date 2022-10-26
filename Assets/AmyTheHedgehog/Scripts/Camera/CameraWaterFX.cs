using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
namespace Amy
{

	public class CameraWaterFX : MonoBehaviour
	{
        public bool isInWater = false;

        public Color waterFogColor;
        public float waterNear = 0.0f;
        public float waterFar = 32.0f;

        public WaterDistortPostProcess waterDistort;

    	// Start is called before the first frame update
    	void Start()
    	{
    	    
    	}

        void setFogParams()
        {

        }

    	// Update is called once per frame
    	void Update()
    	{
    	    if(isInWater)
            {
                GameManager.Instance.systemData.AUDIO_GameSFXMixer.SetFloat("LowPass", 1000.0f);
                waterDistort.enabled = true;
            }
            else
            {
                GameManager.Instance.systemData.AUDIO_GameSFXMixer.SetFloat("LowPass", 22000.00f);
                waterDistort.enabled = false;
            }
    	}
	}

}
