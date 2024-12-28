using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class HotelElevator : MonoBehaviour
	{

		public int targetExit = 0;
		public string targetScene = "";

		public Animator mAnimator;
		bool elevatorCalled = false;
		bool elevatorOpen = false;

		public AudioSource elevatorBody;
		public AudioSource elevatorBell;
		public AudioClip sfx_moving;
		public AudioClip sfx_opening;
		public AudioClip sfx_bell;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		public void CallElevator()
        {
			if (elevatorCalled)
				return;

			elevatorCalled = true;

			Timing.RunCoroutine(waitForElevator());
		}

		public void closeDoor()
        {
			if (!elevatorCalled || !elevatorOpen)
				return;

			Timing.RunCoroutine(doElevatorWarp());
		}

		IEnumerator<float> doElevatorWarp()
        {

			yield return Timing.WaitForSeconds(0.5f);
			mAnimator.Play("Close");
			elevatorBody.clip = sfx_opening;
			elevatorBody.loop = false;
			elevatorBody.Play();

			yield return Timing.WaitForSeconds(1.0f);

			PlayerManager.Instance.lastExit = targetExit;
			GameManager.Instance.loadScene(targetScene);
		}

		IEnumerator<float> waitForElevator()
        {
			elevatorBody.clip = sfx_moving;
			elevatorBody.loop = true;
			elevatorBody.Play();

			yield return Timing.WaitForSeconds(Random.Range(1.0f, 5.0f));

			elevatorBell.PlayOneShot(sfx_bell);

			yield return Timing.WaitForSeconds(0.25f);

			mAnimator.Play("Open");

			elevatorBody.clip = sfx_opening;
			elevatorBody.loop = false;
			elevatorBody.Play();

			yield return Timing.WaitForSeconds(1.0f);
			elevatorOpen = true;

		}
	}
}
