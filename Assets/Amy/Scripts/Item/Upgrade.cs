using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Upgrade : MonoBehaviour
	{
		protected Animator mAnimator;
		protected const PlayableCharacter character = PlayableCharacter.Amy;

		// Start is called before the first frame update
		void Start()
	    {
			if (!mAnimator)
				mAnimator = GetComponent<Animator>();

			if (playerHasItem())
			{
				Die();
			}

			if (PlayerManager.Instance.currentCharacter != character)
				Die();

		}

		protected virtual bool playerHasItem()
        {
			return true;
        }

		protected virtual void doItemGetScene()
        {
			//Do a little item get dance? X3
        }

        protected void OnTriggerEnter(Collider other)
        {
			Player pl = other.GetComponent<Player>();

			if (!pl || playerHasItem())
				return;

			doItemGetScene();
		}

        // Update is called once per frame
        void Update()
	    {
	        
	    }

		void Die()
        {
			Destroy(gameObject);
		}
	}
}
