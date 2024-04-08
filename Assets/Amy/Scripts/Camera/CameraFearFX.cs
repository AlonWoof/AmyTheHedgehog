using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;


//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CameraFearFX : MonoBehaviour
	{

		PostProcessVolume vol;
		Player player;


		// Start is called before the first frame update
		void Start()
		{
			vol = GetComponentInChildren<PostProcessVolume>();

			vol.enabled = false;

		}

		// Update is called once per frame
		void Update()
		{
			if (!vol)
				return;

			if(!player)
            {
				player = PlayerManager.Instance.getPlayer();
				return;
			}

			PlayerStatus pStats = player.getStatus();

			if(pStats.checkStatusEffect(PlayerStatusFX.Scared))
            {
				vol.enabled = true;
				vol.weight = Mathf.Lerp(vol.weight, 1.0f, Time.deltaTime * 4.0f);
			}
			else
            {
				vol.weight = Mathf.Lerp(vol.weight, 0.0f, Time.deltaTime * 4.0f);

				if(vol.weight < 0.001f)
					vol.enabled = false;
			}
		}
	}
}
