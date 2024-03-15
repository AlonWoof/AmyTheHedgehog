using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public enum DamageType
    {
		None,
		Neutral,
		Fire,
		Electric,
		Water,
		Poison,
		Crush
    }

	//Whether the damage ignores nothing, the player, or the enemy
	public enum DamageTeam
    {
		None,
		Player,
		Enemy
    }

	public class Damage : MonoBehaviour
	{

		public float damageAmount = 0.0f;
		public DamageType damageType = DamageType.Neutral;

		public bool hurtsPlayer = false;
		public bool hurtsEnemy = false;

		public GameObject source;
		public GameObject collisionFX;

		public UnityEvent onContact;

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
			//Hitbox hb = other.GetComponent<Hitbox>();

			//if (!hb)
			//	return;

			Debug.Log("HIT! " + other.gameObject.name);

			//if (hb.damageTeam == damageTeam)
			//	return;

			//hb.onTakeDamage(this);

			if (other.GetComponent<Player>() && hurtsPlayer)
			{
				bool success = other.GetComponent<Player>().takeDamage(this);

				if (!success)
					return;

				onContact.Invoke();

				if (collisionFX)
				{
					GameObject inst = GameObject.Instantiate(collisionFX);
					inst.transform.position = Vector3.Lerp(transform.position, other.transform.position, 0.5f);
				}
			}

			if (other.GetComponent<Enemy>() && hurtsEnemy)
			{
				bool success = other.GetComponent<Enemy>().takeDamage(this);

				if (!success)
					return;

				onContact.Invoke();

				if (collisionFX)
				{
					GameObject inst = GameObject.Instantiate(collisionFX);
					inst.transform.position = Vector3.Lerp(transform.position, other.transform.position, 0.5f);
				}
			}
		}
    }
}
