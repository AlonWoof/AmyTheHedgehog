using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerMode : MonoBehaviour
	{
		protected Player mPlayer;
		protected Rigidbody mRigidBody;

		protected void getBaseComponents()
        {
			mPlayer = GetComponentInChildren<Player>();
			mRigidBody = GetComponentInChildren<Rigidbody>();
        }
	}
}
