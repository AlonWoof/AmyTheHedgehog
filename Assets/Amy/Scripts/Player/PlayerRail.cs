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
		public float mSpeed = 0.0f;

		public float animSpeed;
		public float leanLeftRight = 0.0f;

		Vector3 offset;

		public GameObject clothModel;
		public GameObject mountPoint;

		public float railProgress = 0.0f;
		public float railDistance = 0.0f;
		public int currentRailNode = 0;

		const float mountPointHeight = 1.1f;
		const float mountPointForward = 0.165f;

		public float speedMult = 0.0f;

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

			if(clothModel)
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

			if(Input.GetButtonDown("Jump"))
            {
				DismountRailJump();
				return;
			}

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
			calcPosition();
			swingAnimation();
		}

        private void LateUpdate()
        {
			//checkIfNextNode();
			//lockToCorrectPosition();
			//mPlayer.tpc.centerBehindPlayer();
			

		}


        public void MountRail(Rail r, Vector3 pos)
        {
			mRail = r;
			railDistance = 0.0f;
			mSpeed = 0.0f;

			mPlayer.transform.SetParent(null);
			mountPoint.transform.position = pos;
			mountPoint.transform.rotation = transform.rotation;
			transform.position = mountPoint.transform.position - offset;
			mPlayer.tpc.centerBehindPlayer();

			mPlayer.transform.SetParent(mountPoint.transform);

			mPlayer.mRigidBody.isKinematic = true;
			mSpeed = mPlayer.speed.magnitude;
			animSpeed = Mathf.Clamp01(mSpeed / mPlayer.mParam.railSpeed);
			mPlayer.mAnimator.Play("Rail");
			triggerNodeEvent();

		}

		public void getRailDirectionAndSpeedMult()
        {
			Vector3 back = mRail.getPosOnRailFromDistance(railDistance - 0.5f);
			Vector3 forward = mRail.getPosOnRailFromDistance(railDistance + 0.5f);

			speedMult = (back.y - forward.y) * 2.0f;


			Vector3 dir = Helper.getDirectionTo(back, forward);

			dir.y = 0;

			mountPoint.transform.rotation = Quaternion.LookRotation(dir.normalized, Vector3.up);
        }

		public void DismountRail()
        {
			mPlayer.mRigidBody.isKinematic = false;
			mPlayer.transform.SetParent(null);
			mPlayer.changeCurrentMode(PlayerModes.NORMAL);
			mPlayer.mAnimator.Play("Airborne");
			mPlayer.isOnGround = false;
			mPlayer.framesAirborne = 10;
			mRail.checkForPlayerTimeout = 1.0f;

			mPlayer.acceleration.y = mSpeed * 0.5f;
			mPlayer.acceleration.z = mSpeed * 0.5f;
		}

		public void DismountRailJump()
        {
			mPlayer.mRigidBody.isKinematic = false;
			mPlayer.transform.SetParent(null);
			mPlayer.changeCurrentMode(PlayerModes.NORMAL);
			mPlayer.mAnimator.Play("Jump");
			mPlayer.Jump(true);
			mPlayer.acceleration.y += mSpeed * 0.25f;
			mPlayer.acceleration.z = mSpeed * 0.5f;
			mPlayer.isOnGround = false;
			mPlayer.framesAirborne = 10;
			mRail.checkForPlayerTimeout = 1.0f;
		}

		void calcSpeed()
		{

			mSpeed += (speedMult * mPlayer.mParam.railSpeed) * Time.fixedDeltaTime;

			float desiredAnimSpeed = Mathf.Clamp01(mSpeed / mPlayer.mParam.railSpeed);

			animSpeed = Mathf.Lerp(animSpeed, desiredAnimSpeed, Time.fixedDeltaTime * 3.0f);

			mPlayer.mAnimator.SetFloat("animSpeed", animSpeed);
	

			railDistance += mSpeed * Time.fixedDeltaTime;

		}

		void calcPosition()
        {

			if (railDistance > mRail.totalDistance)
			{
				DismountRail();
				return;

			}

			transform.position = mountPoint.transform.position - (Vector3.up * mountPointHeight) - (transform.forward * mountPointForward);
			mountPoint.transform.position = mRail.getPosOnRailFromDistance(railDistance);

			getRailDirectionAndSpeedMult();


		}




		void oldCalcSpeed()
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

			int node = mRail.getNodeIndexFromDistance(railDistance);

			if ((mRail.points.Length - 1) < node)
				return null;

			if (mRail.points[node] == null)
				return null;

			return mRail.points[node];
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

			int node = mRail.getNodeIndexFromDistance(railDistance) + 1;

			if ((mRail.points.Length-1) < node)
				return null;

			if (mRail.points[node] == null)
				return null;

			return mRail.points[node];
        }

		void triggerNodeEvent()
        {
			if (!mRail)
				return;

			if ((mRail.events.Length - 1) < railProgress)
				return;

			if (mRail.events[currentRailNode] == null)
				return;

			mRail.events[currentRailNode].Invoke();
		}

		void swingAnimation()
        {
			float leaning = leanLeftRight + Mathf.Sin(Time.time * 2.0f) * 0.2f;
			leaning = Mathf.Clamp(leaning, -1.0f, 1.0f);

			float fac = Mathf.Lerp(-30, 30, (leaning + 1.0f) * 0.5f);

			//mPlayer.tpc.centerBehindPlayer();
			
			//mountPoint.transform.rotation = Quaternion.LookRotation(mountPoint.transform.forward) * Quaternion.Euler(0, 0, fac);
		}
	}
}
