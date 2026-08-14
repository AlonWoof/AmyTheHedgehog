using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class InivisiCookieEffect : ItemEffect
	{
	    // Start is called before the first frame update
	    void Start()
	    {
			PlayerManager.Instance.addStealthCamo(mPlayer);
	    }
	
	}
}
