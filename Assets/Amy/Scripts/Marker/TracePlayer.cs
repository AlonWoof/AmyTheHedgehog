using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jason Haden */
namespace Amy
{

	public class TracePlayer : MonoBehaviour
	{

        Player mPlayer;

        public bool snapToGround = false;
        public LayerMask groundSnapMask;

        public bool copyRotation = false;
        public bool snapToWaterSurface = false;

    	// Start is called before the first frame update
    	void Start()
    	{
    	    
    	}

    	// Update is called once per frame
    	void Update()
    	{
    	    if(mPlayer == null)
            {
                mPlayer = FindObjectOfType<Player>();
                return;
            }

            transform.position = mPlayer.transform.position;

            if (copyRotation)
                transform.rotation = mPlayer.transform.rotation;

            if(snapToWaterSurface)
            {
                Vector3 pos = transform.position;
                //pos.y = mPlayer.getWaterYPos();
                transform.position = pos;
            }

            if(snapToGround)
            {
                RaycastHit hitInfo = new RaycastHit();

                if(Physics.Linecast(mPlayer.transform.position + Vector3.up,mPlayer.transform.position - Vector3.up * 1000,out hitInfo, groundSnapMask))
                {
                    transform.position = hitInfo.point;
                }
            }
    	}
	}

}
