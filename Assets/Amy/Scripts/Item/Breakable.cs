using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class Breakable : MonoBehaviour
	{

		public DamageType damageType;
		public GameObject deathFX;

		public UnityEvent onDamage;

	    // Start is called before the first frame update
	    void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }

		public bool takeDamage(Damage dmg)
        {
			if(dmg.damageType != damageType && damageType != DamageType.None)
            {
				return false;
            }

			onDamage.Invoke();
			return true;
		}
	}
}
