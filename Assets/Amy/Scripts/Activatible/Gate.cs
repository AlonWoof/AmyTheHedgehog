using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Gate : MonoBehaviour
	{

		public bool isOpen;
		public GameObject openCutscene;

		float sceneTime = 3.0f;

		public Animator mAnimator;

		public bool useStoryFlag = false;
		public string storyFlag;

		public AudioSource gateAudio;
		public AudioClip openMoveSound;
		public AudioClip openFinishSound;

        private void Start()
        {

			if(storyFlag == "")
            {
				storyFlag = gameObject.name + gameObject.transform.position.x + gameObject.transform.rotation.y;
            }

            if(useStoryFlag)
            {
				isOpen = PlayerManager.Instance.getStoryFlag(storyFlag);
            }
        }

        // Update is called once per frame
        void Update()
        {
            mAnimator.SetBool("isOpen", isOpen);


		}

		public void openGate()
        {
			if(openCutscene)
            {
				Timing.RunCoroutine(doOpenCutscene());
            }
			else
            {
				isOpen = true;

				if (useStoryFlag)
					PlayerManager.Instance.setStoryFlag(storyFlag, isOpen);
			}
        }

		public void closeGate()
        {
			isOpen = false;

			if (useStoryFlag)
				PlayerManager.Instance.setStoryFlag(storyFlag, isOpen);
		}

		IEnumerator<float> doOpenCutscene()
        {
			GameManager.Instance.disableInput();
			yield return Timing.WaitForSeconds(1.0f);
			openCutscene.SetActive(true);
			yield return Timing.WaitForSeconds(0.5f);
			isOpen = true;
			yield return Timing.WaitForSeconds(0.5f);

			gateAudio.PlayOneShot(openMoveSound);

			yield return Timing.WaitForSeconds(1.0f);
			gateAudio.Stop();

			yield return Timing.WaitForSeconds(2.0f);

			openCutscene.SetActive(false);
			GameManager.Instance.enableInput();

			if (useStoryFlag)
				PlayerManager.Instance.setStoryFlag(storyFlag, isOpen);

		}
    }
}
