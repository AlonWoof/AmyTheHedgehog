using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

/* Copyright 2021 Jason Haden */
namespace Amy
{

    public class ActorLookAtController : MonoBehaviour
    {
        public EyeTarget eyeLook;
        public LookAtIK headLook;

        public bool lookingAtTarget = false;
        public Vector3 desiredLookAt = Vector3.zero;


        // Start is called before the first frame update
        void Start()
        {
            eyeLook = GetComponentInChildren<EyeTarget>();
            headLook = GetComponentInChildren<LookAtIK>();
        }

        // Update is called once per frame
        void LateUpdate()
        {



            if (!headLook)
                return;

            if (!lookingAtTarget)
            {
                headLook.solver.headWeight = Mathf.Lerp(headLook.solver.headWeight, 0.0f, Time.deltaTime * 16.0f);
            }
            else
            {
                headLook.solver.headWeight = Mathf.Lerp(headLook.solver.headWeight, 0.75f, Time.deltaTime * 16.0f);

                headLook.solver.target.transform.position = Vector3.Lerp(headLook.solver.target.transform.position, desiredLookAt, Time.deltaTime * 8.0f);
            }


            if (!eyeLook)
                return;

            if (!lookingAtTarget)
            {
                eyeLook.overrideLook = false;
            }
            else
            {
                eyeLook.overrideLook = true;
                eyeLook.eyeTarget.transform.position = Vector3.Lerp(eyeLook.eyeTarget.transform.position, desiredLookAt, Time.deltaTime * 16.0f);
            }

            if (Input.GetKey(KeyCode.L))
            {
                //eyeTarget.transform.position = GameManager.Instance.mainCamera.transform.position;
                desiredLookAt = GameManager.Instance.mainCamera.transform.position;
                lookingAtTarget = true;
            }
        }
    }

}