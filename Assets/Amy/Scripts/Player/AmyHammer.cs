using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AmyHammer : PlayerMode
	{

		public GameObject hammerModel;
		public Transform hammerNode;
		public GameObject hammerChargeFX;
		public GameObject hammerSpinFX;

		public Damage hammerDamage;

		public float currentScale = 0.0f;

		public float chargeFXCurrentOpacity = 0.0f;
		public float chargeFXDesiredOpacity = 0.0f;
		public bool isAttacking = false;

		public Material glowFXMaterial;

	    // Start is called before the first frame update
	    void Start()
	    {
			getBaseComponents();

			hammerNode = mPlayer.getBoneByName("weapon");

			if (!hammerNode)
				return;

			hammerDamage = hammerNode.GetComponentInChildren<Damage>();


			if (!hammerModel)
				return;

			foreach(Renderer r in GetComponentsInChildren<Renderer>())
            {
				foreach(Material m in r.materials)
                {
					if(m.shader.name.ToLower().Contains("chargeaura"))
                    {
						glowFXMaterial = m;
                    }
                }
            }

		}

        private void Update()
        {

			currentScale = mPlayer.mAnimator.GetFloat("hammerScale");

			if (!PlayerManager.Instance.hasHammer)
				currentScale = 0.0f;

		}

        // Update is called once per frame
        void LateUpdate()
	    {
			if (!hammerModel || !hammerNode)
				return;

			hammerNode.transform.localScale = Vector3.one * currentScale;


			if (currentScale <= 0.001f)
				hammerModel.SetActive(false);
			else
				hammerModel.SetActive(true);

			updateChargeFX();
			updateHammerSpinFX();
		}

		public void updateHammerSpinFX()
        {
			if (!mPlayer)
				return;

			hammerSpinFX.SetActive(mPlayer.isHammerSpin);
        }

		public void enableHurtbox()
        {
			if (!hammerDamage)
				return;

			hammerDamage.gameObject.SetActive(true);
        }

		public void disableHurtbox()
        {
			if (!hammerDamage)
				return;

			hammerDamage.gameObject.SetActive(false);
		}

		void setChargeFXOpacity(float charge)
        {
			if (!glowFXMaterial)
				return;

			charge = Mathf.Clamp01(charge);

			glowFXMaterial.SetFloat("_Opacity", charge);
		}

		public void updateChargeFX()
        {
			if (mPlayer.hammerJumpCharge < 0.01f)
			{
				chargeFXCurrentOpacity = Mathf.Lerp(chargeFXCurrentOpacity, 0.0f, 0.15f);
				chargeFXDesiredOpacity = 0.0f;
			}
			else if (mPlayer.hammerJumpCharge > 0.9f)
			{
				chargeFXDesiredOpacity = 1.0f;
			}


			chargeFXCurrentOpacity = Mathf.Lerp(chargeFXCurrentOpacity, chargeFXDesiredOpacity, Time.deltaTime * 9.0f);
			setChargeFXOpacity(chargeFXCurrentOpacity);

		}
  
    }
}
