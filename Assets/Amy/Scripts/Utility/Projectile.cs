using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    [RequireComponent(typeof(Rigidbody))]
    public class Projectile : MonoBehaviour
    {

        public Rigidbody body;
        public GameObject onCollisionFX;

        public float speed = 6.0f;
        public float spread = 0.0f;

        public LayerMask colMask;
        public DamageTeam team;

        // Start is called before the first frame update
        void Start()
        {
            body = GetComponent<Rigidbody>();

            switch(team)
            {
                case DamageTeam.None:
                    colMask = LayerMask.GetMask("Collision", "PlayerHitbox", "EnemyHitbox");
                    break;
                case DamageTeam.Player:
                    colMask = LayerMask.GetMask("Collision", "EnemyHitbox");
                    break;
                case DamageTeam.Enemy:
                    colMask = LayerMask.GetMask("Collision", "PlayerHitbox");
                    break;
            }

            

        }

        private void Update()
        {
            gameObject.layer = LayerMask.NameToLayer("Projectile");
            doContactRaycast();
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            body.velocity = transform.forward * speed;
        }

        void doContactRaycast()
        {
            Vector3 start = transform.position;
            Vector3 end = transform.position + ((transform.forward * (speed * Time.fixedDeltaTime)) * 1.5f);

            RaycastHit hitInfo = new RaycastHit();

            Debug.DrawLine(start, end, Color.red);

            if(Physics.Linecast(start,end, out hitInfo))
            {  
                transform.position = hitInfo.point;
                speed *= 0.25f;
                body.velocity = Vector3.zero;
                //Die();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Collision"))
                Die();

            if (other.GetComponent<Hitbox>())
                Die();
        }

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Collision"))
            {
                Die();
            }

            if (collision.collider.GetComponent<Hitbox>())
            {
                
            }
        }

        void Die()
        {
            if (onCollisionFX != null)
                SpawnFX();

            Destroy(gameObject);
        }

        void SpawnFX()
        {
            GameObject fx = GameObject.Instantiate(onCollisionFX);
            fx.transform.position = transform.position;
            fx.transform.rotation = transform.rotation;
        }
    }
}