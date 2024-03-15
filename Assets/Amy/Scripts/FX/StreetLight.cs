using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class StreetLight : MonoBehaviour
	{
		// Start is called before the first frame update

		public GameObject onModel;
		public GameObject offModel;

		SceneInfo scn;

	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        if(!scn)
            {
				scn = FindObjectOfType<SceneInfo>();
				return;
            }

			if(PlayerManager.Instance.isNightTime)
            {
				onModel.SetActive(true);
				offModel.SetActive(false);
            }
			else
            {
				onModel.SetActive(false);
				offModel.SetActive(true);
			}
	    }
	}
}
