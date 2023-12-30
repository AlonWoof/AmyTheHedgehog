using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class TestEnemy : MonoBehaviour
	{

		public Enemy mEnemy;
		public GameObject bullet;
		public GameObject deathExplosion;

		public Transform muzzlePoint;

		public float attackRange = 16.0f;
		public float attackTimer = 3.0f;

		public CoroutineHandle thinkHandle;
		public CoroutineHandle currentActionHandle;

		public Vector3 currentDirection;
		public Vector3 homeDirection;

		void Awake()
		{
			homeDirection = transform.forward;

		}

		// Start is called before the first frame update
		void Start()
	    {
	        
	    }
	
	    // Update is called once per frame
	    void Update()
	    {
			if (attackTimer > 0.0f)
				attackTimer -= Time.deltaTime;


			if(thinkHandle == null)
            {
				thinkHandle = Timing.RunCoroutine(think());
            }
			else if (!thinkHandle.IsRunning || !thinkHandle.IsValid)
            {
				thinkHandle = Timing.RunCoroutine(think());
			}

			transform.rotation = Quaternion.Lerp(transform.rotation,Quaternion.LookRotation(currentDirection.normalized, Vector3.up), Time.deltaTime * 16.0f);
		}

		public IEnumerator<float> think()
		{

			if (getClosestPlayerInRange())
			{
				Player target = getClosestPlayerInRange();

				currentDirection = Helper.getDirectionTo(transform.position, target.transform.position + Vector3.up * 0.5f);

				if(attackTimer < 0.0f)
                {
					fireShot();
					attackTimer = Random.Range(0.1f, 2.0f);
                }
			}
			else
            {
				attackTimer = 3.0f;
				currentDirection = homeDirection;
			}



			yield return Timing.WaitForSeconds(0.1f);
		}

		public Player getClosestPlayerInRange()
		{
			float closest = attackRange * 1.5f;

			Player ret = null;

			foreach (Player p in FindObjectsOfType<Player>())
			{
				float dst = Vector3.Distance(transform.position, p.transform.position + Vector3.up * 0.5f);
				if (dst < closest)
				{
					ret = p;
					closest = dst;
				}
			}

			return ret;
		}

		void fireShot()
		{
			GameObject inst = GameObject.Instantiate(bullet);
			inst.transform.position = muzzlePoint.transform.position;
			inst.transform.rotation = transform.rotation;
		}



		public void Die()
        {
			GameObject inst = GameObject.Instantiate(deathExplosion);
			inst.transform.position = transform.position;
			inst.transform.rotation = transform.rotation;

			Destroy(gameObject);
        }
		
	}
}
