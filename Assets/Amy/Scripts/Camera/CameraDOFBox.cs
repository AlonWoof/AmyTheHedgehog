using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CameraDOFBox : MonoBehaviour
	{

		DepthOfField dof;
		PostProcessVolume vol;


		public bool fxEnabled = false;
		public float focusDist = 0.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
			vol = GetComponentInChildren<PostProcessVolume>();

			vol.enabled = false;
			dof = vol.profile.GetSetting<DepthOfField>();
			
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			if (!vol)
				return;

			vol.enabled = fxEnabled;
			dof.focusDistance.value = focusDist;
	    }
	}
}
