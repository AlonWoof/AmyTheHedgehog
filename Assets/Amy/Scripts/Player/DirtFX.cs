using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class DirtFX : MonoBehaviour
	{

		public PlayableCharacter mChara;
		public List<Material> materials;
		public float desiredDirt = 0.0f;
		float dirtLevel = 0.0f;

		PlayerStatus pStats = null;

		// Start is called before the first frame update
		void Start()
		{
			findAllMats();
		}

		// Update is called once per frame
		void Update()
		{
			if(pStats == null)
            {
				pStats = PlayerManager.Instance.getCharacterStatus(mChara);
				return;
			}

			dirtLevel = Mathf.Lerp(dirtLevel, desiredDirt, Time.deltaTime * 2.0f);

			updateDirt();
		}

		void findAllMats()
		{
			materials = new List<Material>();

			foreach (Renderer r in GetComponentsInChildren<Renderer>())
			{
				foreach (Material m in r.materials)
				{
					if(m.shader.name.ToLower().Contains("body"))
                    {
						materials.Add(m);

					}
				}
			}
		}

		void updateDirt()
        {
			if(pStats.checkStatusEffect(PlayerStatusFX.Dirty))
            {
				desiredDirt = 0.75f;
            }
			else
            {
				desiredDirt = 0.0f;

				if (desiredDirt < 0.01f)
					desiredDirt = 0.0f;
			}

			foreach (Material m in materials)
			{
				m.SetFloat("_Dirtiness", dirtLevel);
			}
		}
	}
}
