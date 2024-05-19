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
			if(player.speed.z > 2.0f)
            {
				excitement = Mathf.Lerp(excitement, 3.0f, Time.deltaTime * 2.0f);
            }
			else
            {
				excitement = Mathf.Lerp(excitement, 0.5f, Time.deltaTime * 0.1f);
            }

			PlayerStatus pstats = PlayerManager.Instance.getCurrentPlayerStatus();

			float fac = (pstats.currentMood / pstats.maxMood);

			excitement *= fac;

			excitement = Mathf.Clamp(excitement, 0.25f, 3.0f);

        }
	}
}
