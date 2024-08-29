using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AimReticule : MonoBehaviour
	{

		SlingshotAimer aimer;
		public CanvasGroup alphaGroup;

		bool slingshotMode = false;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        if(!aimer)
            {

            }
	    }

        private void LateUpdate()
        {


			if (!aimer)
			{
				aimer = FindObjectOfType<SlingshotAimer>(true);
				transform.position = new Vector3(0, -1000000.0f, 0);
				alphaGroup.alpha = Mathf.Lerp(alphaGroup.alpha, 0, Time.deltaTime * 8.0f);
				return;
			}

			if (!aimer.gameObject.activeInHierarchy)
            {
				transform.position = new Vector3(0, -1000000.0f, 0);
				alphaGroup.alpha = Mathf.Lerp(alphaGroup.alpha, 0, Time.deltaTime * 8.0f);
				slingshotMode = false;
				return;
			}



			Vector3 screenPos = GameManager.Instance.mainCamera.WorldToScreenPoint(aimer.target_node.transform.position);

			if (Vector3.Distance(transform.position, screenPos) > 1000.0f)
				transform.position = screenPos;

			if (aimer.gameObject.activeInHierarchy && !slingshotMode)
			{
				slingshotMode = true;
				transform.position = screenPos;
			}

			if (alphaGroup.alpha > 0.99f)
				alphaGroup.alpha = 1.0f;
			else
            {
				alphaGroup.alpha = Mathf.Lerp(alphaGroup.alpha, 1.0f, Time.deltaTime * 8.0f);
			}

			transform.position = Vector3.Lerp(transform.position, screenPos, Time.deltaTime * 16.0f);


        }
    }
}
