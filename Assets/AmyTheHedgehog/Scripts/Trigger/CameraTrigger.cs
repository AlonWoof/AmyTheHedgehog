using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

namespace Amy
{
    public class CameraTrigger : MonoBehaviour
    {
        public CinemachineVirtualCamera vCam;


        // Start is called before the first frame update
        void Start()
        {
            vCam.enabled = false;
            vCam.Priority = -1000;
        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            Player pl = other.GetComponent<Player>();

            if (!pl)
                return;

            vCam.enabled = true;
            vCam.Priority = 1000;
        }

        private void OnTriggerExit(Collider other)
        {
            Player pl = other.GetComponent<Player>();

            if (!pl)
                return;

            vCam.enabled = false;
            vCam.Priority = -1000;
        }
    }
}