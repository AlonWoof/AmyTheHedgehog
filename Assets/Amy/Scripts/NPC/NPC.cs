using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

/* Copyright 2024 Jennifer Haden */
namespace Amy
{

	public class NPC : MonoBehaviour
	{
        public Vector3 mDirection;
        const float rotationSpeed = 3.0f;

        public Message AmyMessage;
        public Message CreamMessage;

        public Talker tk;

        public Transform headNode;
        public ActorLookAtController lookAt;
        public NPCPath path;
        public float lookAtRange = 4.0f;
        

    	// Start is called before the first frame update
    	void Awake()
    	{
            mDirection = transform.forward;

            if(!path)
                path = transform.parent.GetComponentInChildren<NPCPath>();

            if(!headNode)
            {
                foreach(Transform t in GetComponentsInChildren<Transform>())
                {
                    if(t.gameObject.name.ToLower().Contains("head"))
                    {
                        headNode = t;
                    }
                }

                if(!headNode)
                {
                    GameObject inst = new GameObject("head");
                    inst.transform.SetParent(transform);
                    inst.transform.position = transform.position;
                    inst.transform.rotation = transform.rotation;
                    headNode = inst.transform;
                }
            }
        }

    	// Update is called once per frame
    	void Update()
    	{

            if (!PlayerManager.Instance.getPlayer())
                return;

            if (!lookAt)
                return;

            Player pl = PlayerManager.Instance.getPlayer();

            float dst = Vector3.Distance(pl.transform.position + Vector3.up * 0.5f, transform.position);

            if(dst < lookAtRange)
            {
                lookAt.lookingAtTarget = true;
                lookAt.desiredLookAt = pl.transform.position + (Vector3.up * (pl.mParam.height * 0.75f));
            }
            else
            {
                lookAt.lookingAtTarget = false;
                lookAt.desiredLookAt = transform.position + transform.forward + Vector3.up;
            }
        }

        private void LateUpdate()
        {

           // transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mDirection, Vector3.up), Time.deltaTime * rotationSpeed);
        }

        public void turnLookAt(Vector3 lookPos)
        {
            Vector3 mPos = transform.position;
            mPos.y = 0.0f;

            lookPos.y = 0.0f;

            mDirection = Helper.getDirectionTo(mPos, lookPos);
        }

        public void turnLookAt(Transform target)
        {
            turnLookAt(target.transform.position);
        }

        public void genericTalk()
        {
            Timing.RunCoroutine(doGenericTalk().CancelWith(gameObject));        
        }

        IEnumerator<float> doGenericTalk()
        {
            Player mPlayer = PlayerManager.Instance.getPlayer();
            Message msg = AmyMessage;

            if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Cream)
                msg = CreamMessage;

            CoroutineHandle msgProc = UIManager.Instance.messageBox.showMessageBox(msg);

            if (path)
                path.disableMovement();

            bool wasNormal = false;

            if (mPlayer.currentMode == PlayerModes.NORMAL)
            {
                mPlayer.changeCurrentMode(PlayerModes.LISTENING);
                wasNormal = true;
            }

            while(msgProc.IsRunning)
            {
                yield return 0f;
            }

            if (path)
                path.enableMovement();

            if (wasNormal)
                mPlayer.changeCurrentMode(PlayerModes.NORMAL);
        }


    }

}
