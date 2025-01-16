using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
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
			GameManager.Instance.playSystemSound(GameManager.Instance.systemData.AUDIO_itemGetJingle, 0.5f);

			Invoke("Die", 5.0f);
		}

	}
}
