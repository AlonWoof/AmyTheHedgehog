using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public enum MOOD_GRAPHIC
    {
		MOOD_FULL,
		MOOD_NEUTRAL,
		MOOD_LOW,
		MOOD_ITAI,
		MOOD_KOWAII,
		MOOD_ECCHI
    }

	public class MoodIndicatorFace : MonoBehaviour
	{
		public Image mImage;
		public Sprite[] sprites;
		MOOD_GRAPHIC currentMood;


		// Start is called before the first frame update
		void Start()
	    {
	        
	    }

        // Update is called once per frame
        void Update()
	    {
			PlayerStatus pStats = PlayerManager.Instance.getCurrentPlayerStatus();
			Player player = PlayerManager.Instance.getPlayer();

			if (player == null || pStats == null)
				return;

			float fac = pStats.currentMood / pStats.maxMood;

			if (fac > 0.9f)
				currentMood = MOOD_GRAPHIC.MOOD_FULL;
			else if(fac > 0.25f)
				currentMood = MOOD_GRAPHIC.MOOD_NEUTRAL;
			else
				currentMood = MOOD_GRAPHIC.MOOD_LOW;

			if(player.currentMode == PlayerModes.HURT)
				currentMood = MOOD_GRAPHIC.MOOD_ITAI;

			if (player.currentMode == PlayerModes.RUBBING)
				currentMood = MOOD_GRAPHIC.MOOD_ECCHI;

			if (pStats.checkStatusEffect(PlayerStatusFX.Scared))
				currentMood = MOOD_GRAPHIC.MOOD_KOWAII;

			mImage.sprite = sprites[(int)currentMood];
		}

		
	}
}
