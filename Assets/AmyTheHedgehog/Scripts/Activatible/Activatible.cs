using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Amy
{

    public class Activatible : MonoBehaviour
    {

        public UnityEvent onActivate;
        public float range = 3.0f;
        public bool directionDependent = false;
        public string interactAction = "Talk";

        public Vector3 offset = Vector3.zero;
        public Vector3 reticuleOffset = Vector3.zero;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        public void Activate(Player player, bool distanceOverride = false)
        {
            onActivate.Invoke();
        }
    }
}
