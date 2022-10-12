using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
namespace Amy
{

	public class Exit : MonoBehaviour
	{

        public int exitNumber = 0;

        public Transform altCheckpoint;

        private void OnDrawGizmosSelected()
        {
            gameObject.name = "Exit #" + exitNumber;

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f + transform.forward);
        }

        // Start is called before the first frame update
        void Start()
    	{
    	    
    	}

    	// Update is called once per frame
    	void Update()
    	{
    	    
    	}
	}

}
