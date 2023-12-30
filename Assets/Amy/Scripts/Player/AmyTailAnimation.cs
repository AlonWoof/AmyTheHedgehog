using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AmyTailAnimation : PlayerMode
	{

		public Player player;
		public float excitement = 1.0f;
		public float mood = 1.0f;

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
			mAnimator.SetFloat("mood", mood);
	    }

		void calcMoodAndExcitement()
        {
			if(player.speed.z > 2.0f)
            {
				excitement = Mathf.Lerp(excitement, 3.0f, Time.deltaTime * 2.0f);
            }
			else
            {
				excitement = Mathf.Lerp(excitement, 0.2f, Time.deltaTime * 0.1f);
            }

			mood = Mathf.Lerp(mood, PlayerManager.Instance.AmyStatus.currentMood/PlayerManager.Instance.AmyStatus.maxMood, Time.deltaTime * 1.2f);

        }
	}
}
