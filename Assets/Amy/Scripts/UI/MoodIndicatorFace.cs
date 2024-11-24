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
		public Image moodCircle;
		public Sprite[] sprites;
		MOOD_GRAPHIC currentMood;

		public Gradient moodGradient;
		public Color color_happy;
		public Color color_neutral;
		public Color color_sad;
		public Color color_pain;
		public Color color_aroused;
		public Color color_Scared;
		public Color color_Sick;
		public Color color_Tired;

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


			float fac = pStats.getCondition();

			if (fac > 0.9f)
			{
				currentMood = MOOD_GRAPHIC.MOOD_FULL;
				//moodCircle.CrossFadeColor(Color.Lerp(Color.white, color_happy, 0.5f), 1.0f, true, false);
			}
			else if (fac > 0.25f)
			{
				currentMood = MOOD_GRAPHIC.MOOD_NEUTRAL;
				//moodCircle.CrossFadeColor(Color.Lerp(Color.white, color_neutral, 0.5f), 1.0f, true, false);
			}
			else
			{
				currentMood = MOOD_GRAPHIC.MOOD_LOW;
				//moodCircle.CrossFadeColor(Color.Lerp(Color.white, color_sad, 0.5f), 1.0f, true, false);
			}

			if (player.currentMode == PlayerModes.HURT)
			{
				//moodCircle.CrossFadeColor(color_pain, 0.2f, true, false);
				currentMood = MOOD_GRAPHIC.MOOD_ITAI;
			}

			if (player.currentMode == PlayerModes.RUBBING)
			{
				//moodCircle.CrossFadeColor(Color.Lerp(Color.white, color_aroused, 0.5f), 5.0f, true, false);
				currentMood = MOOD_GRAPHIC.MOOD_ECCHI;
			}

			if (pStats.checkStatusEffect(PlayerStatusFX.Scared))
				currentMood = MOOD_GRAPHIC.MOOD_KOWAII;

			mImage.sprite = sprites[(int)currentMood];
		}

		
	}
}
