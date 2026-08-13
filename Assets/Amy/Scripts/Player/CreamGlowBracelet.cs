using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2026 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CreamGlowBracelet : PlayerMode
	{
		public GameObject braceletModel;
		public Light braceletLight;
		public Light charLight;

	    // Start is called before the first frame update
	    void Start()
	    {
			getBaseComponents();

			if (!mPlayer)
				return;
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			if (!braceletModel || !braceletLight)
				return;

			if (!PlayerManager.Instance.hasGlowBracelet)
			{
				braceletModel.SetActive(false);
			}
			else
            {
				braceletModel.SetActive(true);

				if(mPlayer.getStatus().checkVibe(VibeType.Dark))
                {
					braceletLight.enabled = true;
					braceletLight.intensity = Mathf.Lerp(braceletLight.intensity, 0.75f, Time.deltaTime);
					braceletLight.range = Mathf.Lerp(braceletLight.range, 60.0f, Time.deltaTime);
                }
				else
                {
					braceletLight.intensity = Mathf.Lerp(braceletLight.intensity, 0.0f, Time.deltaTime);
					braceletLight.range = Mathf.Lerp(braceletLight.range, 0.0f, Time.deltaTime);

					if (braceletLight.intensity < 0.01f)
						braceletLight.enabled = false;
				}
			}
	    }
	}
}
