using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jennifer Haden */
namespace Amy
{

	public class TracePlayer : MonoBehaviour
	{

        Player mPlayer;
        public GameObject overrideObject;

        public bool snapToGround = false;
        public LayerMask groundSnapMask;

        public bool copyRotation = false;
        public bool snapToWaterSurface = false;
        public bool onlyXZ = false;

    	// Start is called before the first frame update
    	void Start()
    	{
    	    
    	}

    	// Update is called once per frame
    	void Update()
    	{

            Transform target;

    	    if(mPlayer == null && !overrideObject)
            {
                mPlayer = FindObjectOfType<Player>();
                return;
            }

            if (!overrideObject)
                target = mPlayer.transform;
            else
                target = overrideObject.transform;


            Vector3 pos = target.position;

            if (onlyXZ)
                pos.y = transform.position.y;

            transform.position = pos;

            if (copyRotation)
                transform.rotation = target.rotation;

            if(snapToWaterSurface)
            {
                pos = transform.position;
                //pos.y = mPlayer.getWaterYPos();
                transform.position = pos;
            }

            if(snapToGround)
            {
                RaycastHit hitInfo = new RaycastHit();

                if(Physics.Linecast(target.position + Vector3.up, target.position - Vector3.up * 1000,out hitInfo, groundSnapMask))
                {
                    transform.position = hitInfo.point;
                }
            }
    	}
	}

}
