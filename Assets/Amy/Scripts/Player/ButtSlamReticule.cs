using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ButtSlamReticule : MonoBehaviour
	{

		public Player mPlayer;
		public float mScale = 0.0f;
		public float desiredScale = 0.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }

        void Update()
        {
			if (mPlayer.mChara != PlayableCharacter.Cream)
				return;

			if (!mPlayer)
				mPlayer = PlayerManager.Instance.mPlayerInstance;

			bool showReticule = true;

			if (mPlayer.currentMode != PlayerModes.FLY)
				showReticule = false;

			if (mPlayer.getAltitudeFromGround() < 3.0f)
				showReticule = false;

			if (mPlayer.tpc.currentAngle.x < 30.0f)
				showReticule = false;

			if (showReticule)
			{
				desiredScale = 1.0f;

				Vector3 start = mPlayer.transform.position;
				Vector3 end = (mPlayer.transform.position + Vector3.down * 128.0f);



				RaycastHit hitInfo = new RaycastHit();

				if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
				{
					transform.position = hitInfo.point + (hitInfo.normal * 0.1f);
					transform.rotation = Quaternion.LookRotation(transform.forward, hitInfo.normal);

					Debug.DrawLine(start, hitInfo.point, Color.blue, 1.1f);
				}
				else
				{
					transform.position = Vector3.down * 5000.0f;
				}
			}
			else
			{
				desiredScale = 0.0f;
			}

		}

        // Update is called once per frame
        void LateUpdate()
        {
			mScale = Mathf.Lerp(mScale, desiredScale, Time.deltaTime * 8.0f);
			transform.localScale = new Vector3(mScale, 1.0f, mScale);
	    }


	}
}
