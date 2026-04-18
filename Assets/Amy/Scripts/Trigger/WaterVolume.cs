using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jennifer Haden */
namespace Amy
{

    public class WaterVolume : MonoBehaviour
    {

        CameraWaterFX fx = null;
        Collider camCollider = null;

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.GetComponentInChildren<CameraWaterFX>())
            {
                fx = other.gameObject.GetComponentInChildren<CameraWaterFX>();
                fx.isInWater = true;
                camCollider = other;
            }

            Rigidbody r = other.GetComponent<Rigidbody>();

            if (r)
            {
                if (r.velocity.magnitude > 3.0f)
                {
                    GameObject inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_ActorWaterSplashFX);
                    inst.transform.position = other.transform.position;
                    inst.transform.rotation = other.transform.rotation;
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if(other == camCollider)
            {
                if(fx)
                {
                    fx.isInWater = true;
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other == camCollider)
            {
                if (fx)
                {
                    fx.isInWater = false;
                }
            }
        }
    }

}
