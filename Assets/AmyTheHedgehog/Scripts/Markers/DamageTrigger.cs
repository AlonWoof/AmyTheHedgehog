using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Amy
{
    public class DamageTrigger : MonoBehaviour
    {
        public bool hurtsPlayer = false;
        public bool hurtsEnemy = false;
        public bool hurtsProp = false;

        public float damageAmount = 0.15f;

        public Vector3 lastPos;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            lastPos = transform.position;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Player>())
            {
                if(hurtsPlayer)
                {
                    other.GetComponent<Player>().onTakeDamage(damageAmount, lastPos, 10.0f);
                }
            }
        }
    }
}