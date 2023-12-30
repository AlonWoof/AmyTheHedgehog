using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
namespace Amy
{

	public class CameraTracer : MonoBehaviour
	{

        Camera mCamera;

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

            transform.position = mCamera.transform.position;
            transform.rotation = mCamera.transform.rotation;
        }

    }

}
