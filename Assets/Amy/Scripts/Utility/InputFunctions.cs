using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class InputFunctions
    {

        static float controllerDeadzone = 0.1f;
        
        public static float getLeftAnalogX()
        {
            if (GameManager.Instance.usingController)
            {
                if (Mathf.Abs(Input.GetAxis("Left Analog X")) > controllerDeadzone)
                    return Input.GetAxis("Left Analog X");
            }
            else
            {
                if (Mathf.Abs(Input.GetAxisRaw("Horizontal")) > controllerDeadzone)
                    return Input.GetAxisRaw("Horizontal");
            }

            return 0f;
        }

        public static float getLeftAnalogY()
        {
            if (GameManager.Instance.usingController)
            {
                if (Mathf.Abs(Input.GetAxis("Left Analog Y")) > controllerDeadzone)
                    return Input.GetAxis("Left Analog Y");
            }
            else
            {
                if (Mathf.Abs(Input.GetAxisRaw("Vertical")) > controllerDeadzone)
                    return Input.GetAxisRaw("Vertical");
            }

            return 0f;
        }

        public static float getRightAnalogX()
        {
            return 0f;
        }

        public static float getRightAnalogY()
        {
            return 0f;
        }
        
    }

}