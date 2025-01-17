using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2025 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class AIPlayerFollow : PlayerMode
	{

		AIPlayer aiplayer;
		public float sisterDistance = 0.0f;
		public float sisterHorizontalDistance = 0.0f;
		public float sisterVerticalDistance = 0.0f;
		public Vector3 sisterDirection = Vector3.forward;

		public bool migi = false;
		public bool hidari = false;

	    // Start is called before the first frame update
	    void Awake()
	    {
			getBaseComponents();
			aiplayer = mPlayer.gameObject.GetComponent<AIPlayer>();

		}
	
	    // Update is called once per frame
	    void Update()
	    {
			aiplayer.handleVirtualButtons();
			evalauteDistanceFromSister();
			followSister();
			checkForDie();
		}

		void checkForDie()
        {
			if(mPlayer.currentMode == PlayerModes.KILLED)
            {
				mPlayer.transform.position = mPlayer.lastSafeGroundPosition;
				mPlayer.getStatus().currentHealth = mPlayer.getStatus().maxHealth;

				mPlayer.changeCurrentMode(PlayerModes.NORMAL);
            }
        }

		void evalauteDistanceFromSister()
        {
			Player pl = PlayerManager.Instance.mPlayerInstance;

			if (!pl)
				return;

			Vector3 mpos = transform.position;
			Vector3 tpos = pl.transform.position;

			if (migi)
				tpos += (pl.transform.right * 0.5f);

			if(hidari)
				tpos -= (pl.transform.right * 0.5f);

			sisterDistance = Vector3.Distance(mpos, tpos);
			sisterVerticalDistance = tpos.y - mpos.y;

			mpos.y = 0;
			tpos.y = 0;

			sisterHorizontalDistance = Vector3.Distance(mpos, tpos);
			sisterDirection = Helper.getDirectionTo(mpos, tpos);
		}

		public void followSister()
        {
			Player pl = PlayerManager.Instance.mPlayerInstance;

			aiplayer.desiredVirtualAnalogX = 0.0f;
			aiplayer.desiredVirtualAnalogY = 0.0f;

			if (sisterHorizontalDistance > 1.5f)
            {
				aiplayer.desiredVirtualAnalogY = sisterDirection.z;
				aiplayer.desiredVirtualAnalogX = sisterDirection.x;
			}

			checkJump();

		}
		
		public void checkJump()
        {
			if (mPlayer.currentMode != PlayerModes.NORMAL)
				return;

			if(sisterVerticalDistance > 1.5f)
            {
				if(!aiplayer.virtualJumpHeld)
					aiplayer.pressJump();
			}

			if(sisterHorizontalDistance > 2.0f && canJumpObstacle())
            {
				if (!aiplayer.virtualJumpHeld)
					aiplayer.pressJump();
			}

			if (sisterHorizontalDistance > 2.0f && canJumpGap())
			{
				if (!aiplayer.virtualJumpHeld)
					aiplayer.pressJump();
			}

			if (sisterVerticalDistance < 0.5f || mPlayer.framesGrounded > 10)
			{
				if(shouldReleaseJump())
					aiplayer.releaseJump();
            }
        } 

		public bool shouldReleaseJump()
        {

			if (mPlayer.framesGrounded < 5)
				return false;

			Vector3 start = transform.position;
			Vector3 end = start - Vector3.up * 16.0f;

			if (!Physics.Linecast(start, end, mPlayer.mColMask))
				return false;

			return true;
        }

		public bool canJumpObstacle()
        {
			Vector3 start = transform.position + Vector3.up * 0.45f;
			Vector3 end = start + transform.forward * 3.0f;

			RaycastHit hitInfo = new RaycastHit();

			bool front = false;
			bool top = false;

			if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
			{
				if(Vector3.Dot(transform.forward, -hitInfo.normal) > 0.5f)
					front = true;
			}

			start += Vector3.up * 3.0f;
			end += Vector3.up * 3.0f;

			if (!Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
				top = true;

			if (front && top)
				return true;

			return false;
		}

		public bool canJumpGap()
        {
			Vector3 start = (transform.position + Vector3.up * 0.5f) + transform.forward;
			Vector3 end = start + (Vector3.down * 3.0f);

			bool verticalDrop = false;
			bool foundFloor = false;

			RaycastHit hitInfo = new RaycastHit();

			Debug.DrawLine(start, end, Color.magenta, 3.0f);

			if (!Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
            {
				verticalDrop = true;
            }

			if (!verticalDrop)
				return false;

			start = (transform.position + Vector3.up * 0.5f) + (transform.forward * 8.0f);
			end = start + (Vector3.down * 3.0f);

			Debug.DrawLine(start, end, Color.magenta, 3.0f);

			if (Physics.Linecast(start, end, out hitInfo, mPlayer.mColMask))
			{
				if(Vector3.Dot(Vector3.up, hitInfo.normal) > 0.6f)
					foundFloor = true;
			}

			if (verticalDrop && foundFloor)
				return true;

			return false;
        }


	}
}
