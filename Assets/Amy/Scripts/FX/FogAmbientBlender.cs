using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[ExecuteInEditMode]
	public class FogAmbientBlender : MonoBehaviour
	{

		public bool blendEnabled = false;

		[ColorUsage(true, true)]
		public Color light_sky_a;
		[ColorUsage(true, true)]
		public Color light_equator_a;
		[ColorUsage(true, true)]
		public Color light_ground_a;
		public Color fog_color_a;
		public float fog_density_a;

		[ColorUsage(true, true)]
		public Color light_sky_b;
		[ColorUsage(true, true)]
		public Color light_equator_b;
		[ColorUsage(true, true)]
		public Color light_ground_b;
		public Color fog_color_b;
		public float fog_density_b;

		[Range(0.0f, 1.0f)]
		public float blendAmount = 0.0f;

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }
	

	    // Update is called once per frame
	    void Update()
	    {
	        if(blendEnabled)
            {
				RenderSettings.ambientSkyColor = Color.Lerp(light_sky_a, light_sky_b, blendAmount);
				RenderSettings.ambientEquatorColor = Color.Lerp(light_equator_a, light_equator_b, blendAmount);
				RenderSettings.ambientGroundColor = Color.Lerp(light_ground_a, light_ground_b, blendAmount);
				RenderSettings.fogDensity = Mathf.Lerp(fog_density_a, fog_density_b, blendAmount);
				RenderSettings.fogColor = Color.Lerp(fog_color_a, fog_color_b, blendAmount);
			}
			else
            {
				light_sky_a = RenderSettings.ambientSkyColor;
				light_equator_a = RenderSettings.ambientEquatorColor;
				light_ground_a = RenderSettings.ambientGroundColor;
				fog_density_a = RenderSettings.fogDensity;
				fog_color_a = RenderSettings.fogColor;
			}
	    }
	}
}
