using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class CreamEarSpinFX : PlayerMode
	{
		public GameObject earSpinFX;
		public Damage earDamage;


		// Start is called before the first frame update
		void Start()
	    {
			getBaseComponents();

			if (!mPlayer)
				return;

		}

        private void LateUpdate()
        {
			updateEarSpinFX();
		}

        public void updateEarSpinFX()
		{
			if (!mPlayer)
				return;

			earSpinFX.SetActive(mPlayer.isHammerSpin);
		}
	}
}
