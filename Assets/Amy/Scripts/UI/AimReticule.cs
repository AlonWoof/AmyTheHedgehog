using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AimReticule : MonoBehaviour
	{

		SlingshotAimer aimer;
		public CanvasGroup alphaGroup;

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
				aimer = FindObjectOfType<SlingshotAimer>();
				transform.position = new Vector3(0, -1000000.0f, 0);
				alphaGroup.alpha = Mathf.Lerp(alphaGroup.alpha, 0, Time.deltaTime * 8.0f);
				return;
			}

			if (!aimer.gameObject.activeInHierarchy)
            {
				transform.position = new Vector3(0, -1000000.0f, 0);
				alphaGroup.alpha = Mathf.Lerp(alphaGroup.alpha, 0, Time.deltaTime * 8.0f);
				return;
			}

			Vector3 screenPos = GameManager.Instance.mainCamera.WorldToScreenPoint(aimer.target_node.transform.position);

			if (Vector3.Distance(transform.position, screenPos) > 1000.0f)
				transform.position = screenPos;

			

			if (alphaGroup.alpha > 0.99f)
				alphaGroup.alpha = 1.0f;
			else
            {
				alphaGroup.alpha = Mathf.Lerp(alphaGroup.alpha, 1.0f, Time.deltaTime * 8.0f);
			}

			transform.position = Vector3.Lerp(transform.position, screenPos, Time.fixedDeltaTime * 4.0f);


        }
    }
}
