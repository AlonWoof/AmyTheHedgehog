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
