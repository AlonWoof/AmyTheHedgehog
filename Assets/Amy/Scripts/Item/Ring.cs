using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jennifer Haden */
namespace Amy
{

	public class Ring : MonoBehaviour
	{
        public bool hoverAboveGround = false;
        public float hoverHeight = 0.5f;
        public LayerMask mColMask;

        bool isCollected = false;
        public bool tobitiri = false;
        public float timeLeft = 10.0f;
        public Renderer mRenderer;

        public GameObject ringCollectFX;


    	// Start is called before the first frame update
    	void Start()
    	{
            mColMask = LayerMask.GetMask("Collision");

            if(!mRenderer)
                mRenderer = GetComponentInChildren<Renderer>();

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
            if (tobitiri)
                tobitiriUpdate();
    	}

        private void OnTriggerEnter(Collider other)
        {
            if (isCollected)
                return;

            if (!other.GetComponent<Player>())
                return;

            

            Player mPlayer = other.GetComponent<Player>();

            if (mPlayer.mutekiTimer > 0.01f && tobitiri)
                return;

            PlayerManager.Instance.addRings(1);

            //PlayerManager.Instance.getCharacterStatus(mPlayer.mChara).currentHealth += 0.05f;

            GameObject fxinst = GameObject.Instantiate(ringCollectFX);
            fxinst.transform.position = transform.position;
            gameObject.SetActive(false);
        }

        void tobitiriUpdate()
        {
            timeLeft -= Time.deltaTime;

            if (timeLeft < 1.0f)
            {
                mRenderer.enabled = ((Time.frameCount & (1 << 0x1)) != 0);
            }
            else if (timeLeft < 2.0f)
            {
                mRenderer.enabled = ((Time.frameCount & (1 << 0x2)) != 0);
            }
            

            if(timeLeft < 0.0f)
            {
                Destroy(gameObject);
            }
        }



        private void OnDrawGizmosSelected()
        {
            mColMask = LayerMask.GetMask("Collision");

            if (hoverAboveGround)
            {
                RaycastHit hitInfo = new RaycastHit();

                if (Physics.Linecast(transform.position, transform.position - Vector3.up * 32.0f, out hitInfo, mColMask))
                {
                    transform.position = hitInfo.point + Vector3.up * hoverHeight;
                }
            }
        }
    }

}
