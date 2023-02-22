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
		public float leanLeftRight = 0.0f;

		Vector3 offset;

		public GameObject clothModel;
		public GameObject mountPoint;

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

			if (!mountPoint)
				mountPoint = new GameObject("RailMountPoint");

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

			handleInput();


		}

		void handleInput()
        {
			float h = InputFunctions.getLeftAnalogX();
			float v = InputFunctions.getLeftAnalogY();


			if (Mathf.Abs(h) == 0 && Mathf.Abs(v) == 0)
			{
				leanLeftRight = Mathf.Lerp(leanLeftRight, 0.0f, Time.deltaTime * 3.0f);

				return;
			}


			leanLeftRight = Mathf.Lerp(leanLeftRight, h, Time.deltaTime * 3.0f);

		}

        private void FixedUpdate()
        {
			if (!mRail)
				return;

			calcSpeed();
			
			swingAnimation();
		}

        private void LateUpdate()
        {
			checkIfNextNode();
			lockToCorrectPosition();
		}


        public void MountRail(Rail r)
        {
			mRail = r;
			railProgress = 0;

			mountPoint.transform.position = r.points[0].transform.position + Vector3.up * 0.15f;
			transform.position = mountPoint.transform.position - offset;
			mPlayer.tpc.centerBehindPlayer();

			mPlayer.transform.SetParent(mountPoint.transform);

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

			mPlayer.transform.SetParent(null);
		}

		void calcSpeed()
		{
			RailNode currentNode = getCurrentNode();
			RailNode nextNode = getNextNode();


			if (!nextNode || !currentNode)
				return;

			float nSpd = (currentNode.transform.position.y - nextNode.transform.position.y) * mPlayer.mParam.railSpeed;
			mSpeed = Mathf.Lerp(mSpeed, nSpd, Time.fixedDeltaTime * 0.5f);

			Debug.Log("RAIL DIST: " + getProgressBetweenNodes());
			

			//if (getTotalProgress() < 0.1f)
				//mPlayer.tpc.centerBehindPlayer();


			Vector3 dirToNext = Helper.getDirectionTo(currentNode.transform.position, nextNode.transform.position);

			Vector3 moveVec = dirToNext.normalized * mSpeed;

			mPlayer.setVelocityDirectly(mPlayer.mRigidBody.velocity + (moveVec * Time.fixedDeltaTime));

			dirToNext.y = 0;
			transform.rotation = Quaternion.LookRotation(dirToNext.normalized, Vector3.up);
			mPlayer.direction = dirToNext.normalized;

			float desiredAnimSpeed = Mathf.Clamp01(mSpeed / mPlayer.mParam.railSpeed);

			animSpeed = Mathf.Lerp(animSpeed, desiredAnimSpeed, Time.fixedDeltaTime * 3.0f);

			mPlayer.mAnimator.SetFloat("animSpeed", animSpeed);

			//mPlayer.transform.position += moveVec * Time.deltaTime;
		}

		void checkIfNextNode()
        {
			RailNode currentNode = getCurrentNode();
			RailNode nextNode = getNextNode();

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
			RailNode currentNode = getCurrentNode();
			RailNode nextNode = getNextNode();

			if (!nextNode || !currentNode)
				return -1.0f;

			Vector3 currentPos = transform.position + offset;

			float dista = Vector3.Distance(currentNode.transform.position, nextNode.transform.position);
			float distb = Vector3.Distance(currentNode.transform.position, currentPos);

			return (distb / dista);
			
		}

		float getTotalProgress()
        {
			RailNode firstNode = getFirstNode();
			RailNode lastNode = getLastNode();

			if (!firstNode || !lastNode)
				return -1.0f;

			Vector3 currentPos = transform.position + offset;

			float dista = Vector3.Distance(firstNode.transform.position, lastNode.transform.position);
			float distb = Vector3.Distance(firstNode.transform.position, currentPos);

			return (distb / dista);
		}

		void lockToCorrectPosition()
        {

			RailNode currentNode = getCurrentNode();
			RailNode nextNode = getNextNode();

			if (!nextNode || !currentNode)
				return;

			mountPoint.transform.position = Vector3.Lerp(currentNode.transform.position, nextNode.transform.position, getProgressBetweenNodes()) + Vector3.up * 0.15f; ;


			transform.position = mountPoint.transform.position - offset - (Vector3.up * 0.15f);
			

		}

		RailNode getCurrentNode()
		{
			if (!mRail)
				return null;

			if ((mRail.points.Length - 1) < railProgress)
				return null;

			if (mRail.points[railProgress] == null)
				return null;

			return mRail.points[railProgress];
		}

		RailNode getFirstNode()
		{
			if (!mRail)
				return null;

			if (mRail.points[0] == null)
				return null;

			return mRail.points[0];
		}

		RailNode getLastNode()
		{
			if (!mRail)
				return null;

			if (mRail.points[mRail.points.Length - 1] == null)
				return null;

			return mRail.points[mRail.points.Length - 1];
		}

		RailNode getNextNode()
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

		void swingAnimation()
        {
			float leaning = leanLeftRight + Mathf.Sin(Time.time * 2.0f) * 0.2f;
			leaning = Mathf.Clamp(leaning, -1.0f, 1.0f);

			float fac = Mathf.Lerp(-30, 30, (leaning + 1.0f) * 0.5f);


			mountPoint.transform.rotation = mountPoint.transform.rotation * Quaternion.Euler(fac, 0, 0);
        }
	}
}
