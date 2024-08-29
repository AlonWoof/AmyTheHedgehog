using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ReduceMoneyScene : MonoBehaviour
	{
		public CoroutineHandle routineHandle;
		public Text bankText;
		public Text reduceText;
		public Animator mAnimator;
		int bank;
		

		// Start is called before the first frame update
		void Start()
	    {
			
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			
	    }

		public CoroutineHandle startReduceMoney(int amount)
        {
			return Timing.RunCoroutine(doReduceMoney(amount));
        }

		IEnumerator<float> doReduceMoney(int amount)
        {
			int bank = PlayerManager.Instance.getTotalRings();

			bankText.text = bank.ToString("000000");
			reduceText.text = (0 - amount).ToString("00");
			yield return Timing.WaitForSeconds(1.0f);

			PlayerManager.Instance.subtractTotalRings(amount);

			while(amount > 0)
            {
				amount--;
				bank--;


				bank = Mathf.Clamp(bank, 0, 999999999);
				bankText.text = bank.ToString("000000");
				reduceText.text = "-" + amount.ToString("00");

				yield return Timing.WaitForSeconds(0.01f);
            }

			yield return Timing.WaitForSeconds(1.0f);

			mAnimator.Play("Disappear");

			yield return Timing.WaitForSeconds(1.1f);
		}
	}
}
