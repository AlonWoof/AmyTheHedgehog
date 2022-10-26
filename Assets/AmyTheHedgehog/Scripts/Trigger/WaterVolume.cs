using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jason Haden */
namespace Amy
{

    public class WaterVolume : MonoBehaviour
    {

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponentInChildren<CameraWaterFX>())
            {
                other.gameObject.GetComponentInChildren<CameraWaterFX>().isInWater = true;
            }
        }


        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.GetComponentInChildren<CameraWaterFX>())
            {
                other.gameObject.GetComponentInChildren<CameraWaterFX>().isInWater = false;
            }
        }
    }

}
