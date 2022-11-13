using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jason Haden */
namespace Amy
{

	public class NPC : MonoBehaviour
	{
        public Vector3 mDirection;
        const float rotationSpeed = 3.0f;

    	// Start is called before the first frame update
    	void Awake()
    	{
            mDirection = transform.forward;
    	}

    	// Update is called once per frame
    	void Update()
    	{
    	    
    	}

        private void LateUpdate()
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mDirection, Vector3.up), Time.deltaTime * rotationSpeed);
        }

        public void turnLookAt(Vector3 lookPos)
        {
            Vector3 mPos = transform.position;
            mPos.y = 0.0f;

            lookPos.y = 0.0f;

            mDirection = Helper.getDirectionTo(mPos, lookPos);
        }

        public void turnLookAt(Transform target)
        {
            turnLookAt(target.transform.position);
        }


    }

}
