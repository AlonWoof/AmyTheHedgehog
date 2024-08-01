using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jennifer Haden */
namespace Amy
{

	public class Exit : MonoBehaviour
	{

        public int exitNumber = 0;

        public Transform altCheckpoint;

        public Mesh amyMesh;

        private void OnDrawGizmosSelected()
        {

            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position + Vector3.up * 0.5f, transform.position + Vector3.up * 0.5f + transform.forward);

        }

        private void OnDrawGizmos()
        {
            Color drawColor = SystemColors.AmyColor;
            drawColor.a = 0.5f;
            Gizmos.color = drawColor;
            Gizmos.DrawWireMesh(amyMesh, transform.position, transform.rotation * Quaternion.Euler(-90,0,0), Vector3.one * 100.0f);

        }

        void OnValidate()
        {
            gameObject.name = "Exit #" + exitNumber;
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
