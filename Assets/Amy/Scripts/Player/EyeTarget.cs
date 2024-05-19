using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

    public class EyeTarget : MonoBehaviour
    {
        Material eyeMat;

        List<Material> leftEyeMat;
        List<Material> rightEyeMat;

        public bool isMobian = false;
        public bool overrideLook = true;
        public Transform eyeTarget;
        public Transform headBone;

        public bool cutsceneOverrideEnable = false;
        public float cutsceneOverride_x = 0.0f;
        public float cutsceneOverride_y = 0.0f;
        

        public float look_x = 0.0f;
        public float look_y = 0.0f;

        public float look_x_left = 0.0f;
        public float look_x_right = 0.0f;

        public float look_y_left = 0.0f;
        public float look_y_right = 0.0f;

        [Range(-100, 100)]
        public float x_mult = 27.0f;

        [Range(-100, 100)]
        public float y_mult = 52.0f;

        [Range(-100, 100)]
        public float x_mult_right = 27.0f;
        [Range(-100, 100)]
        public float x_mult_left = 27.0f;

        [Range(-100, 100)]
        public float y_mult_right = 52.0f;
        [Range(-100, 100)]
        public float y_mult_left = 52.0f;


        public Vector2 leftEyeRange_min = new Vector2(-1, -1);
        public Vector2 leftEyeRange_max = new Vector2(1, 1);

        public Vector2 rightEyeRange_min = new Vector2(-1, -1);
        public Vector2 rightEyeRange_max = new Vector2(1, 1);

        // Start is called before the first frame update
        void Start()
        {
            findEyeMat();
            findHeadBone();

            if (isMobian)
                findRightLeftEyeMat();

            if (!headBone || !eyeMat)
                return;

            eyeTarget.SetParent(headBone);
            eyeTarget.localPosition = Vector3.forward;
        }

        // Update is called once per frame
        void LateUpdate()
        {

            if (!eyeMat)
                return;

            if (!headBone)
                return;

            if (isMobian)
                updateMobianEyes();
            else
                updateHumanEyes();


            if (!overrideLook)
            {
                look_x = Mathf.Lerp(look_x, 0.0f, Time.deltaTime * 5.0f);
                look_y = Mathf.Lerp(look_y, 0.0f, Time.deltaTime * 5.0f);
            }


            debugInput();
        }

        void updateHumanEyes()
        {
            if (overrideLook)
            {
                look_x = -Mathf.Clamp(eyeTarget.transform.localPosition.x * x_mult, -1.0f, 1.0f);
                look_y = -Mathf.Clamp(eyeTarget.transform.localPosition.y * y_mult, -1.0f, 1.0f);
            }

            eyeMat.SetFloat("_LookX", look_x);
            eyeMat.SetFloat("_LookY", look_y);
        }

        void updateMobianEyes()
        {
            if (overrideLook)
            {
                look_x = -Mathf.Clamp(eyeTarget.transform.localPosition.x, -1.0f, 1.0f);
                look_y = -Mathf.Clamp(eyeTarget.transform.localPosition.y, -1.0f, 1.0f);
            }
            else if(cutsceneOverrideEnable)
            {
                look_x = Mathf.Clamp(cutsceneOverride_x, -1.0f, 1.0f);
                look_y = Mathf.Clamp(cutsceneOverride_y, -1.0f, 1.0f);
            }

            look_x_left = look_x * x_mult_left;
            look_x_right = look_x * x_mult_right;
            look_y_left = look_y * y_mult_left;
            look_y_right = look_y * y_mult_right;


            clampEyes();

            foreach (Material m in leftEyeMat)
            {
                m.SetFloat("_LookX", look_x_left);
                m.SetFloat("_LookY", look_y_left);
            }

            foreach (Material m in rightEyeMat)
            {
                m.SetFloat("_LookX", look_x_right);
                m.SetFloat("_LookY", look_y_right);
            }

        }

        void clampEyes()
        {
            look_x_left = Mathf.Clamp(look_x_left, leftEyeRange_min.x, leftEyeRange_max.x);
            look_y_left = Mathf.Clamp(look_y_left, leftEyeRange_min.y, leftEyeRange_max.y);

            look_x_right = Mathf.Clamp(look_x_right, rightEyeRange_min.x, rightEyeRange_max.x);
            look_y_right = Mathf.Clamp(look_y_right, rightEyeRange_min.y, rightEyeRange_max.y);
        }

        void findEyeMat()
        {
            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                foreach (Material m in r.materials)
                {
                    if (m.shader.name.ToLower().Contains("eye"))
                    {
                        eyeMat = m;
                    }
                }
            }
        }



        void findRightLeftEyeMat()
        {
            leftEyeMat = new List<Material>();
            rightEyeMat = new List<Material>();

            foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
            {
                foreach (Material m in r.materials)
                {
                    if (m.shader.name.ToLower().Contains("eye") && m.name.ToLower().Contains("eye_l"))
                    {
                        leftEyeMat.Add(m);
                    }

                    if (m.shader.name.ToLower().Contains("eye") && m.name.ToLower().Contains("eye_r"))
                    {
                        rightEyeMat.Add(m);
                    }
                }
            }
        }

        void findRightLeftEyeMatEditor()
        {
            leftEyeMat = new List<Material>();
            rightEyeMat = new List<Material>();

            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                foreach (Material m in r.sharedMaterials)
                {
                    if (m.shader.name.ToLower().Contains("eye") && m.name.ToLower().Contains("eye_l"))
                    {
                        leftEyeMat.Add(m);
                    }

                    if (m.shader.name.ToLower().Contains("eye") && m.name.ToLower().Contains("eye_r"))
                    {
                        rightEyeMat.Add(m);
                    }
                }
            }
        }

        void debugInput()
        {
            if (Input.GetKey(KeyCode.L))
            {
                eyeTarget.transform.position = GameManager.Instance.mainCamera.transform.position;
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

        private void OnValidate()
        {

        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR

            if (leftEyeMat == null || rightEyeMat == null)
            {
                    
            }

            if (isMobian)
            {
                //findRightLeftEyeMat();
                //updateMobianEyes();
            }
#endif
        }
    }
}
