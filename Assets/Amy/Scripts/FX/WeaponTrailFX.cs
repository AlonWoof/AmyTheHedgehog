using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class WeaponTrailFX : MonoBehaviour
	{
		public Transform weaponNode;
		public bool isOn = false;

		public List<ParticleSystem> particles;

		// Start is called before the first frame update
		void Start()
	    {
			particles = new List<ParticleSystem>();

			foreach (ParticleSystem p in GetComponentsInChildren<ParticleSystem>())
			{
				particles.Add(p);
			}

			disableFX();
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			lockToWeaponNode();
	    }

        private void LateUpdate()
        {
			lockToWeaponNode();
		}

        private void FixedUpdate()
        {
			lockToWeaponNode();
		}

        void lockToWeaponNode()
        {
			if (!weaponNode)
				return;

			transform.position = weaponNode.transform.position;
			transform.rotation = weaponNode.transform.rotation;
        }

		public void enableFX()
        {
			foreach (ParticleSystem p in particles)
			{
				p.Play();
			}
		}

		public void disableFX()
        {
			foreach (ParticleSystem p in particles)
			{
				p.Stop();
			}
		}

	}
}
