using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jason Haden */
namespace Amy
{

	public class Ring : MonoBehaviour
	{

        public bool hoverAboveGround = false;
        public float hoverHeight = 0.5f;
        public LayerMask mColMask;

        bool isCollected = false;

        public GameObject ringCollectFX;

    	// Start is called before the first frame update
    	void Start()
    	{
            mColMask = LayerMask.GetMask("Collision");

            if (hoverAboveGround)
            {
                RaycastHit hitInfo = new RaycastHit();

                if(Physics.Linecast(transform.position,transform.position - Vector3.up * 32.0f,out hitInfo,mColMask))
                {
                    transform.position = hitInfo.point + Vector3.up * hoverHeight;
                }
            }

    	}

    	// Update is called once per frame
    	void Update()
    	{
    	    
    	}

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected)
                return;

            if (!other.GetComponent<Player>())
                return;

            PlayerManager.Instance.ringCount++;

            GameObject fxinst = GameObject.Instantiate(ringCollectFX);
            fxinst.transform.position = transform.position;
            gameObject.SetActive(false);
        }
    }

}
