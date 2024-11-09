using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;
using UnityEngine.SceneManagement;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public class GameCrasher : MonoBehaviour
	{

		public GameObject textureFucker;

		private void OnTriggerEnter(Collider other)
		{
			if (other.gameObject.GetComponent<Player>())
			{
				Timing.RunCoroutine(doDeathSequence().CancelWith(gameObject), Segment.RealtimeUpdate);
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

	}
}
