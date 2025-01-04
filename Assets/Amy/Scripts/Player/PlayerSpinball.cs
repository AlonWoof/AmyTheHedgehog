using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerSpinball : MonoBehaviour
	{

		public Animator mAnimator;
		public EulerRotation rot;
		public Damage dmg;

		public Player mPlayer;
		public CapsuleCollider mCol;


		public GameObject ballRoot;
		public GameObject skeletonRoot;

		public Renderer bodyRender;
		public Renderer ballRender;

		int BallTimer = 0;

        private void Awake()
        {
			
        }

        // Start is called before the first frame update
        void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			if (!mPlayer)
			{
				mPlayer = transform.parent.GetComponent<Player>();
				return;
			}

			if(!mCol)
            {
				mCol = mPlayer.GetComponent<CapsuleCollider>();
				return;
            }

			if (mPlayer.isBallMode && !ballRoot.activeInHierarchy)
			{
				ballRoot.SetActive(true);
				skeletonRoot.transform.localScale = Vector3.zero;
				//mCol.enabled = false;
			}

			if (!mPlayer.isBallMode && ballRoot.activeInHierarchy)
			{
				ballRoot.SetActive(false);
				skeletonRoot.transform.localScale = Vector3.one;
				//mCol.enabled = true;
			}

			if((BallTimer & 0x11) == 1)
            {
				bodyRender.enabled = true;
				ballRender.enabled = false;
            }
			else
            {
				bodyRender.enabled = false;
				ballRender.enabled = true;
			}

			if (Time.deltaTime > 0.01f)
				BallTimer++;

			if (BallTimer > 300)
				BallTimer = 0;

			float fac = Mathf.Clamp(mPlayer.speed.magnitude, 1.0f, 10.0f) / 10.0f;

			float rotSpeed = Mathf.Lerp(800.0f, 2000.0f, fac);
			float animSpeed = Mathf.Lerp(1.0f, 1.5f, fac);

			//mAnimator.SetFloat("Speed", animSpeed);
			rot.speed = rotSpeed;
	    }



	}
}
