using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.UI;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class BankTransferScene : MonoBehaviour
	{
		public CoroutineHandle routineHandle;

		public Text ringText;
		public Text bankTest;
		public AudioSource soundFX;
        public AudioClip countSound;
        public AudioClip doneSound;

        public Animator mAnimator;

        int rings;
        int bank;

        public void Start()
        {
            rings = PlayerManager.Instance.getRings();
            bank = PlayerManager.Instance.ringBank;

            ringText.text = rings.ToString("000");
            bankTest.text = bank.ToString("000000");

            
        }

        public CoroutineHandle startRingBankTransfer()
        {

            return Timing.RunCoroutine(doRingTransfer());
        }

        IEnumerator<float> doRingTransfer()
        {
            yield return Timing.WaitForSeconds(2.0f);

            while(rings > 0)
            {
                rings--;
                bank++;

                ringText.text = rings.ToString("000");
                bankTest.text = bank.ToString("000000");

                if(!soundFX.isPlaying)
                    soundFX.PlayOneShot(countSound);

                yield return 0f;

            }

            soundFX.PlayOneShot(doneSound);
            PlayerManager.Instance.transferRingsToBank();
            SaveGame.writeSaveGame(0);
            yield return Timing.WaitForSeconds(3.0f);

            mAnimator.Play("Disappear");

            yield return Timing.WaitForSeconds(0.5f);
        }
	}
}
