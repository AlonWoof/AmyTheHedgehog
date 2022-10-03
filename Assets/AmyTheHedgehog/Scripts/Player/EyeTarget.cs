using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2022 Jason Haden */
namespace Amy
{

    public class EyeTarget : MonoBehaviour
    {

        Material leftEyeMat;
        Material rightEyeMat;

        const float eye_min_right = -0.1f;
        const float eye_max_right = 0.57f;

        const float eye_min_left = -0.57f;
        const float eye_max_left = 0.1f;

        public bool overrideLook = true;
        public Transform eyeTarget;
        public Transform headBone;

        public float look_x = 0.0f;
        public float look_y = 0.0f;

        [Range(-100, 100)]
        public float x_mult = 27.0f;

        [Range(-100, 100)]
        public float y_mult = 52.0f;

        // Start is called before the first frame update
        void Start()
        {
            findEyeMat();
            findHeadBone();

            if (!headBone || !leftEyeMat || !rightEyeMat)
                return;

            if (!eyeTarget)
                eyeTarget = new GameObject("eye_target").transform;

            eyeTarget.SetParent(headBone);
            eyeTarget.localPosition = (Vector3.forward * 0.01f);
        }

        // Update is called once per frame
        void LateUpdate()
        {

            if (!leftEyeMat || !rightEyeMat)
                return;

            if (!headBone)
                return;

            if (overrideLook)
            {
                look_x = Mathf.Clamp(eyeTarget.transform.localPosition.x * x_mult, -1.0f, 1.0f);
                look_y = -Mathf.Clamp(eyeTarget.transform.localPosition.y * y_mult, -0.2f, 0.2f);
            }


            leftEyeMat.SetFloat("_LookX", Mathf.Clamp(look_x, eye_min_left, eye_max_left));
            leftEyeMat.SetFloat("_LookY", look_y);

            rightEyeMat.SetFloat("_LookX", Mathf.Clamp(look_x, eye_min_right, eye_max_right));
            rightEyeMat.SetFloat("_LookY", look_y);

            debugInput();
        }

        void findEyeMat()
        {
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                foreach (Material m in r.materials)
                {

                    Debug.Log("MATNAME: " + m.name);

                    if (m.name.ToLower().Contains("eye_l"))
                        leftEyeMat = m;

                    if (m.name.ToLower().Contains("eye_r"))
                        rightEyeMat = m;

                }
            }
        }

        void debugInput()
        {
            if (Input.GetKey(KeyCode.L))
            {
               // eyeTarget.transform.position = GameManager.Instance.mainCamera.transform.position;
                // eyeTarget.transform.position = PlayerManager.Instance.getPlayer().transform.position + Vector3.up * 0.8f;
            }
        }

        void findHeadBone()
        {
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.gameObject.name.ToLower() == "head")
                    headBone = t;
            }
        }
    }

}