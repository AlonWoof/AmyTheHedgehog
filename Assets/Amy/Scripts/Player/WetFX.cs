using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class WetFX : MonoBehaviour
	{
		public List<Material> materials;
		public List<Material> yumeTaisogi;
		float currentWetness = 0.0f;
		public float desiredWetness = 0.0f;

		Player mPlayer;

		// Start is called before the first frame update
		void Start()
		{
			findAllMats();
			mPlayer = GetComponent<Player>();
		}

		// Update is called once per frame
		void Update()
		{

			if(mPlayer)
            {
				if(mPlayer.getWaterDepth() > 0.5f)
                {
					desiredWetness = 1.0f;
					currentWetness = 1.0f;
				}

				if (desiredWetness > 0.0f)
				{
					desiredWetness -= Time.deltaTime * 0.05f;

				}
				else
				{
					desiredWetness = 0.0f;

				}
			}



			currentWetness = Mathf.Lerp(currentWetness, desiredWetness, 0.125f);

			updateWet();
		}

		void findAllMats()
		{
			materials = new List<Material>();
			yumeTaisogi = new List<Material>();

			foreach (Renderer r in GetComponentsInChildren<Renderer>())
			{
				foreach (Material m in r.materials)
				{
					if (m.shader.name.ToLower().Contains("body"))
					{
						materials.Add(m);

					}

					if (m.shader.name.ToLower().Contains("body"))
					{
						materials.Add(m);

					}

					if (m.shader.name.ToLower().Contains("yumeblend"))
					{
						yumeTaisogi.Add(m);
					}
				}
			}
		}

		void updateWet()
		{

			foreach (Material m in materials)
			{
				m.SetFloat("_Wetness", Mathf.Clamp01(currentWetness));
			}

			foreach (Material m in yumeTaisogi)
			{
				m.SetFloat("_Blend", currentWetness);
				m.SetFloat("_SpecularGloss", Mathf.Clamp(currentWetness * 0.2f, 0.05f, 1.0f));
				m.SetFloat("_SpecularPower", currentWetness * 0.3f);
			}
		}
	}
}
