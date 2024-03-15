using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Upgrade_Hammer : Upgrade
	{


	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		protected override bool playerHasItem()
        {
			return PlayerManager.Instance.hasHammer;
        }


        protected override void doItemGetScene()
        {
			//Insert some fancy cutscene or something.
			mAnimator.Play("Disappear");

			PlayerManager.Instance.hasHammer = true;

			Invoke("Die", 5.0f);
		}

	}
}
