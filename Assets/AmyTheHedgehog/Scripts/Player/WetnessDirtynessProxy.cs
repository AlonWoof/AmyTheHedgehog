using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
namespace Amy
{

	public class WetnessDirtynessProxy : MonoBehaviour
	{

        public float dirtLevel = 0.0f;
        public float wetLevel = 0.0f;
        public bool inWater = false;

        const float dryRate = 0.03f;

        public GameObject dripFX;

        List<Material> materials;

    	// Start is called before the first frame update
    	void Start()
    	{
            materials = new List<Material>();

    	    foreach(Renderer r in GetComponentsInChildren<Renderer>())
            {
                foreach(Material m in r.materials)
                {
                    materials.Add(m);
                }
            }
    	}

    	// Update is called once per frame
    	void Update()
    	{

            if(!inWater)
            {
                if(wetLevel > 0.0f)
                {
                    wetLevel -= (Time.deltaTime * dryRate);
                }
                else
                {
                    wetLevel = 0.0f;
                }
            }

            if (Mathf.Abs(dirtLevel) < 0.01f)
                dirtLevel = 0.0f;

            if (Mathf.Abs(wetLevel) < 0.01f)
                wetLevel = 0.0f;


            foreach (Material m in materials)
            {
                m.SetFloat("_Wetness", wetLevel);
                m.SetFloat("_Dirtyness", Mathf.Clamp01(dirtLevel));
            }
    	}


	}

}
