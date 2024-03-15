using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Upgrade_Slingshot : Upgrade
	{


		// Update is called once per frame
		void Update()
		{

		}

		protected override bool playerHasItem()
		{
			return PlayerManager.Instance.hasSlingshot;
		}


		protected override void doItemGetScene()
		{
			//Insert some fancy cutscene or something.
			mAnimator.Play("Disappear");

			PlayerManager.Instance.hasSlingshot = true;

			Invoke("Die", 5.0f);
		}

	}
}
