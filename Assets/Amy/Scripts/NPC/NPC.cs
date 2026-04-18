using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MEC;

/* Copyright 2026 Jennifer Haden */
namespace Amy
{

	public class NPC : MonoBehaviour
	{
        public Vector3 mDirection;
        const float rotationSpeed = 3.0f;

        public SpeakerProfile speakerProfile;

        public Message AmyMessage;
        public Message CreamMessage;

        public bool hasHornyMessage;
        public Message AmyHornyMessage;
        public Message SickMessage;

        public Transform headNode;
        public ActorLookAtController lookAtController;
        public NPCPath path;
        public float lookAtRange = 4.0f;
        public bool lookAtToggle = true;


        public UnityEvent onStartTalk;
        public UnityEvent onEndTalk;

        public bool overridePrint = true;
        public UnityEvent onStartPrint;
        public UnityEvent onEndPrint;

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

        public void enableLook()
        {
            lookAtToggle = true;
        }

        public void disableLook()
        {
            lookAtToggle = false;
        }

        protected void updatelookAt()
        {
            if (!PlayerManager.Instance.getPlayer())
                return;

            if (!lookAtController)
                return;

            Player pl = PlayerManager.Instance.getPlayer();

            float dst = Vector3.Distance(pl.transform.position + Vector3.up * 0.5f, transform.position);

            Vector3 dir = Helper.getHorizontalDirectionTo(transform.position, pl.transform.position).normalized;


            if (dst < lookAtRange && lookAtToggle && Vector3.Dot(transform.forward, dir) > 0.5f)
            {
                lookAtController.lookingAtTarget = true;
                lookAtController.desiredLookAt = pl.transform.position + (Vector3.up * (pl.mParam.height * 0.75f));
            }
            else
            {
                lookAtController.lookingAtTarget = false;
                lookAtController.desiredLookAt = transform.position + transform.forward + Vector3.up;
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

            if(mPlayer.getStatus().checkStatusEffect(PlayerStatusFX.Horny) && hasHornyMessage)
            {
                msg = AmyHornyMessage;
            }

            if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Yume)
            {
                msg = new Message();
                msg.messages.Clear();
                msg.messages.Add("NPC NAME: " + transform.parent.name + 
                    " \n" + "NUMBER OF MESSAGES: " + 
                    (AmyMessage.messages.Count + CreamMessage.messages.Count)
                    + "\n POS: " + transform.position);
            }

            if (PlayerManager.Instance.currentCharacter == PlayableCharacter.Cream)
                msg = CreamMessage;

            if(overridePrint)
            {
                msg.onStartPrint = onStartPrint;
                msg.onEndPrint = onEndPrint;
            }

            CoroutineHandle msgProc = UIManager.Instance.messageBox.showMessageBox(msg, speakerProfile);

            if (path)
                path.disableMovement();


            bool wasNormal = false;

            if (mPlayer.currentMode == PlayerModes.NORMAL)
            {
                mPlayer.changeCurrentMode(PlayerModes.LISTENING);
                wasNormal = true;
            }

            onStartTalk.Invoke();


            while(msgProc.IsRunning)
            {
                yield return 0f;
            }

            if (path)
                path.enableMovement();


            if (wasNormal)
                mPlayer.changeCurrentMode(PlayerModes.NORMAL);

            yield return Timing.WaitForOneFrame;

            onEndTalk.Invoke();
        }

        private void Update()
        {
            updatelookAt();
        }
    }

}
