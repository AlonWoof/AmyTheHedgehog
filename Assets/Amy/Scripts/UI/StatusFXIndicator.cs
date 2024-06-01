using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class StatusFXIndicator : MonoBehaviour
	{
		PlayerStatus pstats;

		public PlayerStatusFX mEffect;
		public CanvasGroup mGroup;


	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			pstats = PlayerManager.Instance.getCurrentPlayerStatus();

			if(pstats.checkStatusEffect(mEffect))
            {
				mGroup.alpha = Mathf.Lerp(mGroup.alpha, 1.0f, 0.15f);
            }
			else
            {
				mGroup.alpha = Mathf.Lerp(mGroup.alpha, 0.0f, 0.15f);

				if(mGroup.alpha < 0.01f)
					gameObject.SetActive(false);
			}
	    }
	}
}
