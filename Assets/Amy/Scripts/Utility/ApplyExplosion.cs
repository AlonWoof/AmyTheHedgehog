using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class ApplyExplosion : MonoBehaviour
	{
		public List<Rigidbody> mBodies;

		public float explosionForce = 10.0f;
		public float explosionRadius = 2.0f;

	    // Start is called before the first frame update
	    void Start()
	    {
	        foreach(Rigidbody r in mBodies)
            {
				//r.velocity = 
				Vector3 velo = Vector3.up * Random.Range(explosionForce * 0.9f, explosionForce * 1.1f);

				r.transform.rotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));

				velo += transform.forward * Random.Range(explosionForce * 0.9f, explosionForce * 1.1f);

				r.velocity = velo;

			}
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
	        
	    }
	}
}
