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
        public float lifetime = 100.0f;

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
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            body.velocity = transform.forward * speed;

            lifetime -= Time.fixedDeltaTime;

            if (lifetime < 0.0f)
                Die();
        }


        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Collision"))
                lifetime = Time.deltaTime * 1.5f;
        }

        private void OnCollisionEnter(Collision collision)
        {

            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Collision"))
            {
                lifetime = Time.deltaTime * 1.5f;
            }

            if (collision.collider.GetComponent<Hitbox>())
            {
                lifetime = Time.deltaTime * 1.5f;
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