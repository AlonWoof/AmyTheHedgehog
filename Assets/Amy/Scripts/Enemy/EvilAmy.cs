using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MEC;
using UnityEngine.SceneManagement;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public enum EvilAmyMode
    {
		Wander,
		Alerted,
		Search,
		Chase
    }

	public class EvilAmy : MonoBehaviour
	{

		const float timeForSearch = 30.0f;
		const float viewRange = 16.0f;
		const float viewFOV = 45.0f;
		const float wanderSpeed = 4.0f;
		const float chaseSpeed = 6.0f;

		public Transform headNode;
		public List<Transform> nodes;
		public Vector3 lastKnownPosition;

		public GameObject textureFucker;
		public BGMData BGM_HURRYUP;

		public AudioClip[] vo_wander;
		public AudioClip[] vo_search;
		public AudioClip[] vo_chase;

		public float timePlayerVisible = 0.0f;
		public float chaseTimeLeft = 5.0f;
		public float timeSpentChasing = 0.0f;

		public NavMeshAgent mAgent;
		public Animator mAnimator;
		public Transform rootNode;
		public AudioSource voice;
		EvilAmyMode currentMode;

		Vector3 currentVelocity;
		Vector3 currentDirection;
		Vector3 lookDirection = Vector3.forward;

		float nodeTimeout = 5.0f;
		float voiceTimeout = 15.0f;

		int lastVoice = 0;

		CoroutineHandle currentModeRoutine;

		// Start is called before the first frame update
		void Start()
	    {
			lookDirection = headNode.transform.forward;
			currentDirection = headNode.transform.forward;

			if (nodes.Count > 0)
			{
				transform.position = nodes[Random.Range(0, nodes.Count)].position;
				mAgent.SetDestination(transform.position);
				nodeTimeout = Random.Range(5.0f, 10.0f);
			}
		}
	
	    // Update is called once per frame
	    void Update()
	    {
			switch (currentMode)
			{
				case EvilAmyMode.Wander:
					wanderUpdate();
					break;

				case EvilAmyMode.Alerted:
					break;

				case EvilAmyMode.Search:
					break;

				case EvilAmyMode.Chase:
					chaseUpdate();
					break;
			}

			if (mAgent.velocity.magnitude > 0.1f)
				currentDirection = mAgent.velocity.normalized;

			rootNode.transform.rotation = Quaternion.LookRotation(currentDirection.normalized);

			mAnimator.SetFloat("Speed", Mathf.Clamp01(mAgent.velocity.magnitude / chaseSpeed));

			if (voiceTimeout > 0.0f)
			{
				voiceTimeout -= Time.deltaTime;
			}
			else
			{
				voiceTimeout = Random.Range(8.0f, 16.0f);
				playRandomVoice();
			}				
		}

		void changeMode(EvilAmyMode newMode)
        {

			if (newMode == currentMode)
				return;

			if (currentModeRoutine.IsValid)
			{
				Timing.KillCoroutines(currentModeRoutine);
			}


			switch (newMode)
            {
				case EvilAmyMode.Wander:
					currentModeRoutine = Timing.RunCoroutine(doStartWander());
					break;
				case EvilAmyMode.Search:
					break;
				case EvilAmyMode.Chase:
					currentModeRoutine = Timing.RunCoroutine(doStartChase());
					break;
            }
        }


		IEnumerator<float> doStartWander()
		{
			//MusicManager.Instance.changeSongs();
			SceneInfo scn = FindObjectOfType<SceneInfo>();
			MusicManager.Instance.changeSongs(scn.bgmData, 1.0f);
			yield return 0f;

			mAgent.SetDestination(transform.position);
			mAgent.speed = wanderSpeed;

			currentMode = EvilAmyMode.Wander;
			nodeTimeout = 8.0f;
		}

		float getModeFOV()
        {
			switch(currentMode)
            {
				case EvilAmyMode.Chase:
					return viewFOV * 0.75f;
				case EvilAmyMode.Search:
					return viewFOV * 1.5f;
				case EvilAmyMode.Wander:
					return viewFOV;
            }

			return viewFOV;
		}

		float getModeViewDist()
		{
			switch (currentMode)
			{
				case EvilAmyMode.Chase:
					return viewRange * 1.5f;
				case EvilAmyMode.Search:
					return viewRange;
				case EvilAmyMode.Wander:
					return viewRange * 0.75f;
			}

			return viewFOV;
		}

		void wanderUpdate()
        {

			Player pl = PlayerManager.Instance.getPlayer();


			if (EnemyHelpers.isPlayerVisible(pl, headNode, getModeFOV(), getModeViewDist()))
			{
				timePlayerVisible += Time.deltaTime;
				lookDirection = Helper.getDirectionTo(transform.position, pl.transform.position);

				if (timePlayerVisible > 1.0f)
				{
					changeMode(EvilAmyMode.Chase);
					return;
				}
			}
			else
			{
				timePlayerVisible = 0.0f;
			}


			if(nodeTimeout > 0.0f && mAgent.velocity.magnitude < 0.1f)
            {
				nodeTimeout -= Time.deltaTime;
			}
			else if(mAgent.velocity.magnitude < 0.1f)
            {
				if(nodes.Count > 0)
                {
					Transform t = nodes[Random.Range(0, nodes.Count)];
					mAgent.SetDestination(t.position);
					nodeTimeout = Random.Range(5.0f, 10.0f);
				}
            }
		}

		void playRandomVoice()
        {
			AudioClip clip = null;

			int rnd = 0;

			switch (currentMode)
            {
				case EvilAmyMode.Chase:
					if (vo_chase.Length > 0)
					{
						rnd = Random.Range(0, vo_chase.Length - 1);

						if (rnd == lastVoice)
							rnd = Random.Range(0, vo_chase.Length - 1);

						voice.volume = 1.0f;
						clip = vo_chase[Random.Range(0, vo_chase.Length - 1)];
					}
					break;
				case EvilAmyMode.Search:
					if (vo_search.Length > 0)
					{
						rnd = Random.Range(0, vo_search.Length - 1);

						if (vo_search.Length > 1)
						{
							if (rnd == lastVoice)
								rnd = Random.Range(0, vo_search.Length - 1);
						}

						voice.volume = 1.0f;
						clip = vo_search[Random.Range(0, vo_search.Length - 1)];
					}
					break;
				case EvilAmyMode.Wander:
					if (vo_wander.Length > 0)
					{
						rnd = Random.Range(0, vo_wander.Length - 1);

						if (vo_wander.Length > 1)
						{
							while (rnd == lastVoice)
								rnd = Random.Range(0, vo_wander.Length - 1);
						}

						voice.volume = 0.75f;
						clip = vo_wander[Random.Range(0, vo_wander.Length - 1)];
					}
					break;
			}

			if (clip == null)
				return;

			lastVoice = rnd;
			voice.pitch = 1.0f;
			//voice.clip = clip;
			voice.Play();
			//voice.time = clip.length;

			//voice.PlayOneShot(clip);
		}

		//FUCKING RUN

		IEnumerator<float> doStartChase()
        {
			Player pl = PlayerManager.Instance.getPlayer();
			MusicManager.Instance.changeSongs(BGM_HURRYUP,0.01f);
			yield return 0f;

			pl.getStatus().setStatusEffect(PlayerStatusFX.Scared);
			pl.mVoice.playVoice(pl.mVoice.scared, true);
			pl.updateExpression();
			currentMode = EvilAmyMode.Chase;
        }

		void chaseUpdate()
        {

			Player pl = PlayerManager.Instance.getPlayer();
			pl.getStatus().scaredTimeLeft = 60.0f;

			float speedBoost = (timeSpentChasing / 60.0f) * 6.0f;

			mAgent.SetDestination(pl.transform.position + Vector3.up * 0.5f + pl.transform.forward);
			mAgent.speed = chaseSpeed + (Mathf.Sin(Time.time) * 3.0f) + speedBoost;

			timeSpentChasing += Time.deltaTime;

			if (EnemyHelpers.isPlayerVisible(pl, headNode, getModeFOV(), getModeViewDist()))
				chaseTimeLeft = 5.0f;
			else
            {
				chaseTimeLeft -= Time.deltaTime;

				if(chaseTimeLeft < 0.0f)
                {
					//Just go to default for now.
					timeSpentChasing = 0.0f;
					changeMode(EvilAmyMode.Wander);
                }
			}

		}

		public void onDamage()
        {
			if (currentMode != EvilAmyMode.Chase)
			{
				changeMode(EvilAmyMode.Chase);
				lastKnownPosition = PlayerManager.Instance.getPlayer().transform.position;
			}
		}

        private void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.GetComponent<Player>())
            {
				Timing.RunCoroutine(doDeathSequence(), Segment.RealtimeUpdate);
            }

        }


        IEnumerator<float> doDeathSequence()
        {
			//ZA WARUDO
			Time.timeScale = 0.0f;
			GameManager.Instance.playerInputDisabled = true;
			GameManager.Instance.cameraInputDisabled = true;

			GameObject.Instantiate(textureFucker);

			MusicManager.Instance.fadeBGM(0.0f, 0.01f);
			MusicManager.Instance.bgm.volume = 0.0f;

			MusicManager.Instance.changeSongs(null, 0.3f);
			yield return Timing.WaitForSeconds(1.0f);
			

			SceneManager.LoadScene("DeathScreen");

		}
	}
}
