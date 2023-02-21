using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
namespace Forest
{

	public class SimpleMomentum : MonoBehaviour
	{

        public Vector3 currentMovement = Vector3.zero;
        public Vector3 desiredMovement = Vector3.zero;

        public float lerpFactor = 6.0f;

    	// Start is called before the first frame update
    	void Start()
    	{
    	    
    	}

    	// Update is called once per frame
    	void Update()
    	{
            if(currentMovement.magnitude > 0.02f)
                transform.position += (currentMovement * Time.deltaTime);

            currentMovement = Vector3.Lerp(currentMovement, desiredMovement, Time.deltaTime * lerpFactor);
    	}
	}

}
