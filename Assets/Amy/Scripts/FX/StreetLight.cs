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

		public bool isOn = false;

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

			if(isOn)
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


		public void turnOn()
        {
			if(!isOn)
            {
				onModel.SetActive(true);
				offModel.SetActive(false);
			}

			isOn = true;
		}

		public void turnOff()
        {
			if (isOn)
			{
				onModel.SetActive(false);
				offModel.SetActive(true);
			}

			isOn = false;
		}
	}
}
