using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	//Don't swim out too far, little lady....
	public class BeachWaterKiller : MonoBehaviour
	{

		public float maxDistance = 1000.0f;
		public float unstableDistance = 500.0f;

		public Player trackedPlayer;
		public GameObject textureFucker;

		// Update is called once per frame
		void Update()
	    {
	        if(!trackedPlayer)
            {
				trackedPlayer = FindObjectOfType<Player>();
				return;
            }

			if (trackedPlayer.currentMode == PlayerModes.NORMAL && trackedPlayer.isOnGround)
            {
				transform.position = trackedPlayer.transform.position;
            }

			float dist = Vector3.Distance(transform.position, trackedPlayer.transform.position);

			//Debug.Log("BEACH DIST: " + dist);

			if (dist > unstableDistance && trackedPlayer.transform.position.y < 16.0f)
            {
				if(Time.frameCount % 60 == 0)
                {
					if (Random.Range(0, 100) < 3)
					{
						transform.position = trackedPlayer.transform.position;
						Timing.RunCoroutine(doDeathSequence(),Segment.RealtimeUpdate, gameObject);
					}
				}
            }

	    }

		IEnumerator<float> doDeathSequence()
		{
			//ZA WARUDO
			Time.timeScale = 0.0f;
			GameManager.Instance.disableInput();

			GameObject.Instantiate(textureFucker);

			MusicManager.Instance.killBGM(true);
			MusicManager.Instance.bgm.volume = 0.0f;

			MusicManager.Instance.changeSongs(null, 0.3f);
			yield return Timing.WaitForSeconds(1.0f);

			SceneManager.LoadScene("DeathScreen");
		}

		private void OnDrawGizmos()
        {
			if (!trackedPlayer)
				return;

			float dist = Vector3.Distance(transform.position, trackedPlayer.transform.position);

			Color mColor = Color.green;

			if (dist > unstableDistance)
				mColor = Color.red;

			mColor.a = 1.0f;

			Gizmos.color = mColor;
			Gizmos.DrawWireSphere(trackedPlayer.debug_jumpStartPos, 1.0f);
			Gizmos.DrawLine(trackedPlayer.transform.position, transform.position);

			
		}

    }
}
