using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AmyHammer : PlayerMode
	{

		public GameObject hammerModel;

		float currentScale = 0.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
			getBaseComponents();
	    }

        private void Update()
        {
			currentScale = mPlayer.mAnimator.GetFloat("hammerScale");
        }

        // Update is called once per frame
        void LateUpdate()
	    {
			if (!hammerModel)
				return;

			hammerModel.transform.localScale = Vector3.one * currentScale;


			if (currentScale <= 0.001f)
				hammerModel.SetActive(false);
			else
				hammerModel.SetActive(true);
		}
	}
}
