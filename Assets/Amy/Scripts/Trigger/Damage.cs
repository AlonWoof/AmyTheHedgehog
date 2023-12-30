using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
		public DamageTeam damageTeam = DamageTeam.None;

		public Vector3 source;

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
			Hitbox hb = other.GetComponent<Hitbox>();

			if (!hb)
				return;

			Debug.Log("HIT! " + other.gameObject.name);

			if (hb.damageTeam == damageTeam)
				return;

			hb.onTakeDamage(this);
        }
    }
}
