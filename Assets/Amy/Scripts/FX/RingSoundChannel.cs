using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class RingSoundChannel : MonoBehaviour
	{
		public AudioSource ringSound;

	    // Start is called before the first frame update
	    void Start()
	    {
	        if(PlayerManager.Instance.ringLeftChannel)
            {
				ringSound.panStereo = -0.5f;
            }
			else
            {
				ringSound.panStereo = 0.5f;
			}

			PlayerManager.Instance.ringLeftChannel = !PlayerManager.Instance.ringLeftChannel;

		}
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
