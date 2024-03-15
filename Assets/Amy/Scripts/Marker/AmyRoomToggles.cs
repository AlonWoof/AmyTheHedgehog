using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AmyRoomToggles : MonoBehaviour
	{

		public GameObject hammer_PikoPiko;
		public GameObject slingshot;
		public GameObject magicCloth;

		void disableAllProps()
		{
			hammer_PikoPiko.SetActive(false);
			slingshot.SetActive(false);
			magicCloth.SetActive(false);
		}

		void refreshProps()
        {
			disableAllProps();

			if (PlayerManager.Instance.hasHammer)
				hammer_PikoPiko.SetActive(true);

			if (PlayerManager.Instance.hasSlingshot)
				slingshot.SetActive(true);

			if (PlayerManager.Instance.hasCloth)
				magicCloth.SetActive(true);
        }

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }


	}
}
