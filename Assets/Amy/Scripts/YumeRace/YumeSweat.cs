using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class YumeSweat : PlayerMode
	{

		WetFX wetfx;
		float wetness = 0.0f;
		float sweatRate = 0.0204f;

		// Start is called before the first frame update
		void Start()
	    {
			getBaseComponents();
			wetfx = mPlayer.GetComponent<WetFX>();

		}
	
	    // Update is called once per frame
	    void Update()
	    {
			if (!mPlayer)
				return;

			if(mPlayer.acceleration.z > 1.0f)
				wetness += ((mPlayer.acceleration.z / 8.0f) * Time.deltaTime) * sweatRate;

			wetness = Mathf.Clamp01(wetness);
			wetfx.desiredWetness = Mathf.Clamp(wetfx.desiredWetness, wetness, 1.0f);
		}
	}
}
