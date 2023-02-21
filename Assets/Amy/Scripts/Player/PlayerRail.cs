using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class PlayerRail : PlayerMode
	{

		public Rail mRail;
		public int railProgress = 0;
		public float mSpeed = 0.0f;

		public float animSpeed;

		Vector3 offset;

		public GameObject clothModel;

        private void OnEnable()
        {
			getBaseComponents();

			if (mPlayer.currentMode != PlayerModes.RAIL)
			{
				enabled = false;
				return;
			}

			if (!clothModel)
				SpawnClothModel();

			clothModel.SetActive(true);
		}

		void SpawnClothModel()
        {
			clothModel = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyPlayerFX.prop_amyCloth);
			clothModel.transform.position = transform.position;
			clothModel.transform.rotation = transform.rotation;
			clothModel.transform.SetParent(transform);

			if (mPlayer.currentMode != PlayerModes.RAIL)
				clothModel.SetActive(false);
		}

        private void OnDisable()
        {
			clothModel.SetActive(false);

		}

        // Start is called before the first frame update
        void Start()
	    {
			getBaseComponents();

			if (mPlayer.currentMode != PlayerModes.RAIL)
			{
				enabled = false;
				return;
			}

			mPlayer.clearAccel();
			mPlayer.clearSpeed();

			offset = Vector3.up * mPlayer.mParam.height;


		}
	
	    // Update is called once per frame
	    void Update()
	    {



	    }

        private void FixedUpdate()
        {
			if (!mRail)
				return;

			calcSpeed();
			checkIfNextNode();


		}

        public void MountRail(Rail r)
        {
			mRail = r;
			railProgress = 0;

			transform.position = r.points[0].transform.position - (offset);
			mPlayer.tpc.centerBehindPlayer();

			mSpeed = mPlayer.speed.magnitude;
			animSpeed = Mathf.Clamp01(mSpeed / mPlayer.mParam.railSpeed);
			mPlayer.mAnimator.Play("Rail");
			triggerNodeEvent();
		}

		public void DismountRail()
        {
			transform.position = mRail.end.transform.position + (offset * 0.25f);
			mPlayer.changeCurrentMode(PlayerModes.NORMAL);
			mPlayer.framesAirborne = 11;
			mPlayer.isOnGround = false;
			//mPlayer.mAnimator.Play("Airborne");
		}

		void calcSpeed()
		{
			Transform currentNode = getCurrentNode();
			Transform nextNode = getNextNode();


			if (!nextNode || !currentNode)
				return;

			float nSpd = (currentNode.transform.position.y - nextNode.transform.position.y) * mPlayer.mParam.railSpeed;
			mSpeed = Mathf.Lerp(mSpeed, nSpd, Time.fixedDeltaTime * 0.5f);

			Debug.Log("RAIL DIST: " + getProgressBetweenNodes());
			lockToCorrectPosition();

			//if (getTotalProgress() < 0.1f)
				//mPlayer.tpc.centerBehindPlayer();


			Vector3 dirToNext = Helper.getDirectionTo(currentNode.transform.position, nextNode.transform.position);

			Vector3 moveVec = dirToNext.normalized * mSpeed;

			mPlayer.setVelocityDirectly(moveVec);

			dirToNext.y = 0;
			transform.rotation = Quaternion.LookRotation(dirToNext.normalized, Vector3.up);
			mPlayer.direction = dirToNext.normalized;

			float desiredAnimSpeed = Mathf.Clamp01(mSpeed / mPlayer.mParam.railSpeed);

			if (getTotalProgress() > 0.8f)
			{
				desiredAnimSpeed = 0;
			}

			animSpeed = Mathf.Lerp(animSpeed, desiredAnimSpeed, Time.fixedDeltaTime * 3.0f);

			mPlayer.mAnimator.SetFloat("animSpeed", animSpeed);

			//mPlayer.transform.position += moveVec * Time.deltaTime;
		}

		void checkIfNextNode()
        {
			Transform currentNode = getCurrentNode();
			Transform nextNode = getNextNode();

			if (!currentNode)
				return;

			if (!nextNode)
			{
				DismountRail();
				return;
			}

			if(Helper.horizontalDistance(transform.position, nextNode.transform.position) < 0.1f)
            {
				
				railProgress++;
				triggerNodeEvent();
			}

		}
		
		float getProgressBetweenNodes()
        {
			Transform currentNode = getCurrentNode();
			Transform nextNode = getNextNode();

			if (!nextNode || !currentNode)
				return -1.0f;

			Vector3 currentPos = transform.position + offset;

			float dista = Vector3.Distance(currentNode.position, nextNode.position);
			float distb = Vector3.Distance(currentNode.position, currentPos);

			return (distb / dista);
			
		}

		float getTotalProgress()
        {
			Transform firstNode = getFirstNode();
			Transform lastNode = getLastNode();

			if (!firstNode || !lastNode)
				return -1.0f;

			Vector3 currentPos = transform.position + offset;

			float dista = Vector3.Distance(firstNode.position, lastNode.position);
			float distb = Vector3.Distance(firstNode.position, currentPos);

			return (distb / dista);
		}

		void lockToCorrectPosition()
        {

			Transform currentNode = getCurrentNode();
			Transform nextNode = getNextNode();

			if (!nextNode || !currentNode)
				return;

			Vector3 mountPoint = Vector3.Lerp(currentNode.position, nextNode.position, getProgressBetweenNodes());

			transform.position = mountPoint - offset;
        }

		Transform getCurrentNode()
		{
			if (!mRail)
				return null;

			if ((mRail.points.Length - 1) < railProgress)
				return null;

			if (mRail.points[railProgress] == null)
				return null;

			return mRail.points[railProgress];
		}

		Transform getFirstNode()
		{
			if (!mRail)
				return null;

			if (mRail.points[0] == null)
				return null;

			return mRail.points[0];
		}

		Transform getLastNode()
		{
			if (!mRail)
				return null;

			if (mRail.points[mRail.points.Length - 1] == null)
				return null;

			return mRail.points[mRail.points.Length - 1];
		}


		Transform getNextNode()
        {
			if (!mRail)
				return null;

			if ((mRail.points.Length-1) < railProgress + 1)
				return null;

			if (mRail.points[railProgress + 1] == null)
				return null;

			return mRail.points[railProgress + 1];
        }

		void triggerNodeEvent()
        {
			if (!mRail)
				return;

			if ((mRail.events.Length - 1) < railProgress)
				return;

			if (mRail.events[railProgress] == null)
				return;

			mRail.events[railProgress].Invoke();
		}
	}
}
