using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ScaryArea : MonoBehaviour
	{
		public float spookFactor = 0.2f;

		

	    // Start is called before the first frame update
	    void Start()
	    {
			if (!PlayerManager.Instance.gameObject.GetComponent<StatusEffect_Scared>())
			{
				StatusEffect_Scared status = PlayerManager.Instance.gameObject.AddComponent<StatusEffect_Scared>();
				status.spookFactor = spookFactor;
			}

			
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
