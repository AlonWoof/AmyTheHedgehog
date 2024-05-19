using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class MenuCamera : MonoBehaviour
    {

        public GameObject aimPoint;
        public GameObject mCam;


        public float degrees = 20.0f;
        public float deg_x = 20.0f;
        public float deg_y = 20.0f;

        public bool keepAboveGround = true;
        public Vector3 camHome;

        // Start is called before the first frame update
        void Start()
        {
            camHome = mCam.transform.localPosition;
        }

        // Update is called once per frame
        void Update()
        {
            handleInput();


            //mCam.transform.LookAt(aimPoint.transform);
        }

        void handleInput()
        {
            //if (!GameManager.Instance.inputEnabled)
            //   return;


            float camX = 0.0f;
            float camY = 0.0f;

            if (!GameManager.Instance.usingController)
            {
                //camX = -Input.GetAxisRaw("Mouse X");
                //camY = -Input.GetAxisRaw("Mouse Y");
            }
            else
            {
                camX = Input.GetAxisRaw("Right Analog X");
                camY = Input.GetAxisRaw("Right Analog Y");
            }

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(deg_y * camY, deg_x * -camX, 0), 0.25f);

            if(keepAboveGround)
            {
                Vector3 start = mCam.transform.position + Vector3.up;
                Vector3 end = start - Vector3.up * 10.0f;
                RaycastHit hitInfo = new RaycastHit();

                LayerMask mask = LayerMask.GetMask("Collision");

                if(Physics.Linecast(start, end, out hitInfo, mask))
                {
                    if (mCam.transform.position.y < hitInfo.point.y + 0.01f)
                    {
                        Vector3 pos = mCam.transform.position;

                        mCam.transform.position = hitInfo.point + Vector3.up * 0.01f;
                    }
                    else
                    {
                        mCam.transform.localPosition = Vector3.Lerp(mCam.transform.localPosition, camHome, Time.deltaTime * 8.0f);
                    }
                }
                else
                {
                    mCam.transform.localPosition = Vector3.Lerp(mCam.transform.localPosition, camHome, Time.deltaTime * 8.0f);
                }

            }
        }
    }
}
