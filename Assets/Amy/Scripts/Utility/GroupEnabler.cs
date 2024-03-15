using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jennifer Haden */
namespace Forest
{

	public class GroupEnabler : MonoBehaviour
	{


        public List<GameObject> objects;

    	// Start is called before the first frame update
    	void Start()
    	{
    	    
    	}

    	// Update is called once per frame
    	void Update()
    	{
    	    
    	}

        public void EnableAll()
        {
            foreach(GameObject o in objects)
            {
                o.SetActive(true);
            }
        }

        public void DisableAll()
        {
            foreach (GameObject o in objects)
            {
                o.SetActive(false);
            }
        }
	}

}
