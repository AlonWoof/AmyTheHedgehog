using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class StealthCamo : PlayerMode
	{
		const float maxTime = 30.0f;

		//30 seconds of invisibility til it breaks
		float timeLeft = 30.0f;
		float veloFactor = 0.0f;
		float noise = 0.0f;

		List<Material> mats;
		List<Shader> originalShaders;


		// Start is called before the first frame update
		void OnEnable()
	    {

			if(!PlayerManager.Instance.getCurrentPlayerStatus().checkStatusEffect(PlayerStatusFX.Invisible))
            {
				enabled = false;
				return;
            }

			getBaseComponents();
			timeLeft = maxTime;

			mats = new List<Material>();
			originalShaders = new List<Shader>();


			foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
			{
				foreach (Material m in r.materials)
				{

					if (isValidShader(m.shader.name))
					{
						mats.Add(m);
						originalShaders.Add(m.shader);
					}
				}
			}

			foreach (Material m in mats)
			{
				m.shader = Shader.Find("StealthCamo");
			}

		}

        private void OnDisable()
        {
			for (int i = 0; i < mats.Count; i++)
			{
				mats[i].shader = originalShaders[i];
			}

			PlayerManager.Instance.getCurrentPlayerStatus().unSetStatusEffect(PlayerStatusFX.Invisible);
		}

		bool isValidShader(string str)
        {
			if (str.ToLower().Contains("unlit"))
				return false;

			if (str.ToLower().Contains("amybody"))
				return true;

			if (str.ToLower().Contains("vertalphacolor"))
				return true;

			if (str.ToLower().Contains("eye"))
				return true;

			if (str.ToLower().Contains("standard"))
				return true;

			return false;
		}

        void animateShaders()
		{
			if (!mPlayer)
				return;

			veloFactor = Mathf.Lerp(veloFactor, (mPlayer.acceleration.z / 4.0f), Time.deltaTime);

			float distort = 0.1f * veloFactor;
			

			if (timeLeft < 10.0f)
			{

				if (Time.frameCount % 8 == 0 && Random.Range(0,100) < 10)
					noise = 3.0f;


				if (timeLeft < 5.0f)
				{
					if (Time.frameCount % 4 == 0 && Random.Range(0, 100) < 50)
						noise = 8.0f;
				}
			}

			foreach (Material m in mats)
			{
				m.SetFloat("_Distortion", distort);
				m.SetFloat("_noiseBlend", noise);
			}

			noise = Mathf.Lerp(noise, 0.0f, 0.125f);

			if (noise < 0.01f)
				noise = 0.0f;
		}

		// Update is called once per frame
		void Update()
	    {
			timeLeft -= Time.deltaTime;

			animateShaders();

			if (timeLeft <= 0.0f)
				enabled = false;
		}
	}
}
