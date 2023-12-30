using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MEC;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Enemy : MonoBehaviour
	{

		public float currentHealth = 10.0f;
		public float maxHealth = 10.0f;
		public float mutekiTimer = 0.0f;
		public bool isDead = false;


		public UnityEvent onTakeDamageEvent;
		public UnityEvent onDieEvent;



	    // Start is called before the first frame update

	
	    // Update is called once per frame
	    void Update()
	    {


			if (mutekiTimer > 0.0f)
			{
				mutekiTimer -= Time.deltaTime;

				if (mutekiTimer <= 0.0f)
				{
					//Stop flashing

					mutekiTimer = 0.0f;
				}
			}
		}




		public void takeDamage(Damage dmg)
        {

			if (isDead)
				return;

			if (dmg.damageTeam == DamageTeam.Enemy)
				return;

			onTakeDamageEvent.Invoke();

			currentHealth -= dmg.damageAmount;
			updateHealth();

			mutekiTimer = 0.5f;

		}



		void updateHealth()
		{
			currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

			if (currentHealth == 0)
			{
				isDead = true;
				onDieEvent.Invoke();
				//DIE
			}

		}
	}
}
