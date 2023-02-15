using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class Spring : MonoBehaviour
    {

        public float power = 15.0f;
        public GameObject springFX;
        public Animator mAnimator;

        private void OnDrawGizmos()
        {
            Gizmos.DrawLine(transform.position, calcSpringEndPos());
        }

        Vector3 calcSpringEndPos()
        {
            Vector3 start = transform.position;
            float springVelocityLeft = power;
            float springDecay = 16.0f;

            Vector3 currentPos = start;

            while (springVelocityLeft > 0.0f)
            {
                currentPos += ((transform.up * springVelocityLeft) * 0.1666666f);
                springVelocityLeft -= springDecay * 0.1666666f;
            }

            return currentPos - transform.up * 1.5f;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            Player p = other.GetComponent<Player>();

            if (!p)
                return;

            p.changeCurrentMode(PlayerModes.SPRING);
            p.GetComponent<PlayerSpringBounce>().setSpringVelocity(transform.up, power);
            p.transform.position = transform.position;

            if(springFX)
            {
                GameObject inst = GameObject.Instantiate(springFX);
                inst.transform.position = transform.position;
                inst.transform.rotation = transform.rotation;
            }

            if (mAnimator)
                mAnimator.Play("spring_bounce");
        }
    }
}
