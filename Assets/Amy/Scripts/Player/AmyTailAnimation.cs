using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AmyTailAnimation : PlayerMode
	{

		public Player player;
		public float excitement = 1.0f;
		public float desiredExcitement = 1.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
			getBaseComponents();
			player = GetComponent<Player>();
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			
			calcMoodAndExcitement();
			mAnimator.SetFloat("excitement", excitement);
	    }

		void calcMoodAndExcitement()
        {
			if(player.acceleration.z > 0.1f)
            {
				float spd = Mathf.Clamp(player.acceleration.z, 0.2f, 8.0f);

				float spdfac = spd / 8.0f;

				desiredExcitement = 3.5f * spdfac;
            }
			else
            {
				desiredExcitement = 0.5f;
            }

			if (player.currentMode == PlayerModes.RUBBING)
				desiredExcitement = 6.0f;

			PlayerStatus pStats = PlayerManager.Instance.getCurrentPlayerStatus();


			desiredExcitement *= pStats.getCondition();

			desiredExcitement = Mathf.Clamp(desiredExcitement, 0.25f, 3.0f);

			excitement = Mathf.Lerp(excitement, desiredExcitement, Time.deltaTime);
        }
	}
}
