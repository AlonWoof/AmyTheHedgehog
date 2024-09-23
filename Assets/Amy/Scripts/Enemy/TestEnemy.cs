using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
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
		public const float minAttackTime = 2.0f;
		public const float maxAttackTime = 5.0f;

		public CoroutineHandle thinkHandle;
		public CoroutineHandle currentActionHandle;

		public Vector3 currentDirection;
		public Vector3 homeDirection;

		public List<Player> playerList;

		void Awake()
		{
			homeDirection = transform.forward;

		}

		// Start is called before the first frame update
		void Start()
	    {
			playerList = new List<Player>();

			foreach (Player p in FindObjectsOfType<Player>())
			{
				playerList.Add(p);
			}
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			if (attackTimer > 0.0f)
				attackTimer -= Time.deltaTime;


			if(thinkHandle == null)
            {
				thinkHandle = Timing.RunCoroutine(think().CancelWith(gameObject));
            }
			else if (!thinkHandle.IsRunning || !thinkHandle.IsValid)
            {
				thinkHandle = Timing.RunCoroutine(think().CancelWith(gameObject));
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
					attackTimer = Random.Range(minAttackTime, maxAttackTime);
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

			if (playerList == null)
			{
				playerList = new List<Player>();

				foreach(Player p in FindObjectsOfType<Player>())
                {
					playerList.Add(p);
                }
			}

			if(playerList.Count == 0)
            {
				foreach (Player p in FindObjectsOfType<Player>())
				{
					playerList.Add(p);
				}

				return null;
			}

			if(!playerList[0])
            {
				playerList = new List<Player>();

				foreach (Player p in FindObjectsOfType<Player>())
				{
					playerList.Add(p);
				}
			}

			if (playerList.Count == 0)
			{
				playerList = new List<Player>();

				foreach (Player p in FindObjectsOfType<Player>())
				{
					playerList.Add(p);
				}
			}


			float closest = attackRange * 1.5f;

			Player ret = null;

			foreach (Player p in playerList)
			{
				float dst = Vector3.Distance(transform.position, p.transform.position + Vector3.up * 0.5f);
				float alt_diff = Mathf.Abs(transform.position.y - p.transform.position.y);

				if (dst < closest && alt_diff < 3.0f)
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
