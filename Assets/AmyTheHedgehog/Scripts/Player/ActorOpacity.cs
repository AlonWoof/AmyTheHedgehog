using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
namespace Forest
{

	public class ActorOpacity : MonoBehaviour
	{
        List<Material> mats;

        [Range(0,1)]
        public float opacity = 1.0f;

    	// Start is called before the first frame update
    	void Start()
    	{
            mats = new List<Material>();
            getAllMaterials();

        }

    	// Update is called once per frame
    	void LateUpdate()
    	{
            refreshOpacity();

        }

        void getAllMaterials()
        {
            mats.Clear();

            foreach(Renderer r in GetComponentsInChildren<Renderer>())
            {
                foreach(Material m in r.materials)
                {
                    mats.Add(m);
                }
            }
        }

        void refreshOpacity()
        {
            foreach(Material m in mats)
            {
                m.SetFloat("_Opacity", opacity);
            }
        }
	}

}
