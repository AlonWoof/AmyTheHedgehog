using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Copyright 2021 Jennifer Haden */
namespace Forest
{

	public class AlignToSurface : MonoBehaviour
	{
        public float offset = 0.0f;
        public bool snapToGround = false;

        public LayerMask snappingMask;

        private void OnValidate()
        {
            if (!snapToGround)
                return;

            Vector3 start = transform.position + Vector3.up * offset;

            Vector3 down = transform.position + Vector3.down;

            RaycastHit hitInfo = new RaycastHit();

            if(Physics.Linecast(start,down,out hitInfo, snappingMask))
            {
                transform.position = hitInfo.point;
                transform.rotation = Quaternion.identity * Quaternion.LookRotation(transform.forward, hitInfo.normal);

                transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * Quaternion.LookRotation(transform.forward, Vector3.up);
            }
        }

        private void  OnDrawGizmosSelected()
        {
            if (!snapToGround)
                return;

            Vector3 start = transform.position + Vector3.up * offset;

            Vector3 down = transform.position + Vector3.down;

            RaycastHit hitInfo = new RaycastHit();

            if (Physics.Linecast(start, down, out hitInfo, snappingMask))
            {
                transform.position = hitInfo.point;
                transform.rotation = Quaternion.identity * Quaternion.LookRotation(transform.forward, hitInfo.normal);

                transform.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal) * Quaternion.LookRotation(Vector3.forward, Vector3.up);
            }
        }
    }

}
