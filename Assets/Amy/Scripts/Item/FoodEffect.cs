using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class FoodEffect : ItemEffect
	{

		public float staminaHealAmount = 5;
		public float healthHealAmount = 0;
		public bool amyFave = false;
		public bool creamFave = false;

	    // Start is called before the first frame update
	    void Start()
	    {
	        if(amyFave && mPlayer.mChara == PlayableCharacter.Amy)
            {
				healthHealAmount *= 2.0f;
				staminaHealAmount *= 2.0f;
				pStats.setStatusEffect(PlayerStatusFX.GoodFood);
				pStats.goodFoodTimeLeft = Helper.minutesToSeconds(5);

			}
			else if(creamFave && mPlayer.mChara == PlayableCharacter.Cream)
            {
				healthHealAmount *= 2.0f;
				staminaHealAmount *= 2.0f;
				pStats.setStatusEffect(PlayerStatusFX.GoodFood);
				pStats.goodFoodTimeLeft = Helper.minutesToSeconds(5);
			}

			pStats.currentHealth += healthHealAmount;
			pStats.currentStamina += staminaHealAmount;

	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
