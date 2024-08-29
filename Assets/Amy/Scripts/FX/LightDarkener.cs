using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class LightDarkener : MonoBehaviour
	{

		public float ratio = 0.25f;
		public float rate = 3.0f;

		List<Light> sceneLights;
		List<Color> sceneLightsColor;

		Color fogColor;
		Color ambientSkyColor;
		Color ambientEquatorColor;
		Color ambientGroundColor;

		// Start is called before the first frame update
		void Start()
	    {
			sceneLights = new List<Light>();
			sceneLightsColor = new List<Color>();

			foreach (Light l in FindObjectsOfType<Light>())
            {
				sceneLights.Add(l);
				sceneLightsColor.Add(l.color);

			}


			fogColor = RenderSettings.fogColor;
			ambientSkyColor = RenderSettings.ambientSkyColor;
			ambientEquatorColor = RenderSettings.ambientEquatorColor;
			ambientGroundColor = RenderSettings.ambientGroundColor;
		}

	
	    // Update is called once per frame
	    void Update()
	    {
			for(int i = 0; i < sceneLights.Count; i++)
            {
				sceneLights[i].color = Color.Lerp(sceneLights[i].color, Color.Lerp(sceneLightsColor[i], Color.black, ratio), Time.deltaTime * rate);
			}

			RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, Color.Lerp(fogColor, Color.black, ratio), Time.deltaTime *  rate);
			RenderSettings.ambientSkyColor = Color.Lerp(RenderSettings.ambientSkyColor, Color.Lerp(ambientSkyColor, Color.black, ratio), Time.deltaTime * rate);
			RenderSettings.ambientEquatorColor = Color.Lerp(RenderSettings.ambientEquatorColor, Color.Lerp(ambientEquatorColor, Color.black, ratio), Time.deltaTime * rate);
			RenderSettings.ambientGroundColor = Color.Lerp(RenderSettings.ambientGroundColor, Color.Lerp(ambientGroundColor, Color.black, ratio), Time.deltaTime * rate);
		
		}
	}
}
