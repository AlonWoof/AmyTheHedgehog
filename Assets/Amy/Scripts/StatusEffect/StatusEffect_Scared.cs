using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class StatusEffect_Scared : StatusEffect
	{

		public float spookFactor = 0.5f;
		public List<LightSource> lights;
		ScaryArea scaryArea;

	    // Start is called before the first frame update
	    void Start()
	    {
			lights = new List<LightSource>();
			duration = float.PositiveInfinity;
			scanForLights();
		}

		protected override void executeEffect()
		{
			bool isSafe = false;
			float closesDist = 32.0f;

			if (duration < 0.001f)
				return;



			if (Time.frameCount % 30 == 0)
			{
				scanForLights();
			}

			if (lights.Count > 0)
			{ 
				foreach(LightSource l in lights)
				{
					float dst = Vector3.Distance(mPlayer.headBoneTransform.position, l.transform.position);

					if (dst < closesDist)
						closesDist = dst;

					if (dst < l.radius)
					{
						isSafe = true;
					}
				}
			}


			if (!isSafe)
            {
				pStats.currentMood -= (spookFactor * Time.deltaTime * (1.0f - (closesDist/32.0f)));

				if(pStats.currentMood <= 0.001f)
                {
					pStats.currentHealth -= (Time.deltaTime * 1.2f);
					mPlayer.updateHealth();
                }
			}


			
        }

		

		void scanForLights()
        {

			if(!FindObjectOfType<ScaryArea>())
            {
				duration = 0.0f;
				return;
            }

			scaryArea = FindObjectOfType<ScaryArea>();

			lights.Clear();

			foreach(LightSource l in FindObjectsOfType<LightSource>())
            {
				lights.Add(l);
            }
        }
	}
}
