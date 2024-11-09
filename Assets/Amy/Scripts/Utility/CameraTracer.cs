using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jennifer Haden */
namespace Amy
{

	public class CameraTracer : MonoBehaviour
	{

        Camera mCamera;
        public bool lockXZ = false;
        public bool rotation = false;

    	// Start is called before the first frame update
    	void Start()
    	{
    	    if(!mCamera)
            {
                mCamera = GameManager.Instance.mainCamera;
            }
    	}

    	// Update is called once per frame
    	void LateUpdate()
    	{
            alignCamera();
        }

        private void Update()
        {
            alignCamera();
        }

        private void FixedUpdate()
        {
            //alignCamera();

        }

        void alignCamera()
        {
            if (!mCamera)
            {
                mCamera = GameManager.Instance.mainCamera;
                return;
            }

            if(!lockXZ)
                transform.position = mCamera.transform.position;
            else
            {
                Vector3 pos = mCamera.transform.position;
                pos.y = transform.position.y;
                transform.position = pos;
            }

            if(rotation)
                transform.rotation = mCamera.transform.rotation;
        }

    }

}
