using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerNPCSwitch : MonoBehaviour
	{

		public PlayableCharacter mChara;
		public PlayerNPCLocation mLocation;

		public GameObject npcObject;

		public UnityEvent onActive;
		public UnityEvent onInactive;

	    // Start is called before the first frame update
	    void Start()
	    {
			npcObject.SetActive(false);

			if (PlayerManager.Instance.currentCharacter == mChara)
				return;

			if(mChara == PlayableCharacter.Amy)
            {
				if (mLocation == PlayerManager.Instance.AmyStatus.npcLocation)
				{
					npcObject.SetActive(true);
					onActive.Invoke();
				}
            }
			else if(mChara == PlayableCharacter.Cream)
            {
				if (mLocation == PlayerManager.Instance.CreamStatus.npcLocation)
				{
					npcObject.SetActive(true);
					onActive.Invoke();
				}
			}
			else
            {
				onInactive.Invoke();
            }
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
