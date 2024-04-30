using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Shower : MonoBehaviour
	{

		public bool isOn = false;
		public ParticleSystem particleEmitter;
		public GameObject fx;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	    
			if(isOn && !particleEmitter.isPlaying)
            {
				
				particleEmitter.Play();
				fx.SetActive(true);
			}
			else if(!isOn && particleEmitter.isPlaying)
            {
				particleEmitter.Stop();
				fx.SetActive(false);
			}
	    }

		

		public void startShowerForPlayer()
        {
			Player pl = PlayerManager.Instance.getPlayer();
			isOn = true;

			Timing.RunCoroutine(takeAShower(pl));
        }

		IEnumerator<float> takeAShower(Player pl)
        {
			WetFX wetfx = pl.GetComponent<WetFX>();
			wetfx.desiredWetness = 1.0f;

			const float cleanDirtRate = 1.0f;

			GameManager.Instance.cutsceneMode = true;
			GameManager.Instance.playerInputDisabled = true;
			yield return Timing.WaitForSeconds(1);
			wetfx.desiredWetness = 1.0f;

			//Time to take a shower.
			//Wower.
			pl.mAnimator.Play("Shower_Start");
			pl.areaDetector.enabled = false;

			yield return Timing.WaitForSeconds(5);

			while (pl.getStatus().dirtiness > 0.0f)
            {
				pl.getStatus().dirtiness -= (Time.deltaTime * cleanDirtRate);
				wetfx.desiredWetness = 1.0f;
				yield return 0f;
			}

			wetfx.desiredWetness = 1.0f;

			pl.getStatus().dirtiness = 0.0f;
			yield return Timing.WaitForSeconds(3);

			pl.mAnimator.CrossFade("Idle", 0.2f);
			pl.updateExpression();
			GameManager.Instance.cutsceneMode = false;
			GameManager.Instance.playerInputDisabled = false;
			pl.areaDetector.enabled = true;

			Timing.WaitForSeconds(1.0f);
			isOn = false;
		}
	}
}
