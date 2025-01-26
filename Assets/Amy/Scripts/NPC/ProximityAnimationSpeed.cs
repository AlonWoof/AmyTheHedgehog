using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ProximityAnimationSpeed : MonoBehaviour
	{

		public Vector2 animationSpeedRange = new Vector2(0.5f, 1.5f);
		public Vector2 playerDistanceRange = new Vector2(4, 16);
		public Animator mAnimator;

		Player pl;
		float animSpeed = 0;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			calculateDistanceFactor();

		}

		bool getPlayer()
        {
			pl = PlayerManager.Instance.mPlayerInstance;

			if (!pl)
				return false;

			return true;

		}

		void calculateDistanceFactor()
        {
			if (!getPlayer())
				return;

			Vector3 mpos = transform.position;
			Vector3 tpos = pl.transform.position;

			mpos.y = 0;
			tpos.y = 0;

			float dst = Vector3.Distance(mpos, tpos);

			dst = Mathf.Clamp(dst, playerDistanceRange.x, playerDistanceRange.y);

			float fac = Helper.remapRange(dst, playerDistanceRange.x, playerDistanceRange.y, 0, 1);

			animSpeed = Mathf.Lerp(animationSpeedRange.x, animationSpeedRange.y, 1.0f - fac);

			mAnimator.SetFloat("animSpeed", animSpeed);
        }

        private void OnGUI()
        {
			Helper.drawDebugText(new Vector2(200, 200), "ANIM SPEED: " + animSpeed);
        }
    }
}
