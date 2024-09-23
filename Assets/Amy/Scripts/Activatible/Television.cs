using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{


	public class Television : MonoBehaviour
	{
		public bool isOn = false;

		public AudioSource sound;
		public AudioSource interfaceSound;
		public VideoPlayer player;
		public GameObject lookTarget;

		public AudioClip onSound;
		public AudioClip offSound;


		public List<VideoClip> shows;
		public int currentShowIndex = 0;

		Material playerMat;

		// Start is called before the first frame update
		void Start()
	    {
	        foreach(Renderer r in GetComponentsInChildren<Renderer>())
            {
				foreach(Material m in r.materials)
                {
					if(m.name.ToLower().Contains("tv_screen"))
                    {
						playerMat = m;
                    }
                }
            }

			player.isLooping = false;

			shows.Shuffle();
			pickNextShow();

			player.time = Random.Range(0.0f, (float)player.clip.length);

			if(!isOn)
            {
				sound.volume = 0.0f;
				playerMat.SetColor("_EmissionColor", Color.black);
			}

			if(PlayerManager.Instance.currentCharacter != PlayableCharacter.Amy && PlayerManager.Instance.AmyNPCLocation == PlayerNPCLocation.WatchTV)
            {
				turnOn();
			}


			if (PlayerManager.Instance.currentCharacter != PlayableCharacter.Cream && PlayerManager.Instance.CreamNPCLocation == PlayerNPCLocation.WatchTV)
			{
				turnOn();
			}

		}

        private void OnEnable()
        {
            
        }

        // Update is called once per frame
        void Update()
	    {


			if (!GameManager.Instance.gamePaused && !PlayerManager.Instance.itemMenuOpen)
			{

				if (player.isPaused)
					player.Play();

				if (player.time > player.clip.length * 0.999f)
					pickNextShow();


			}
			else
            {
				if (!player.isPaused)
					player.Pause();
            }


			handleDebugInput();
		}

		void pickNextShow()
        {
			player.Stop();
			player.clip = shows[currentShowIndex++];

			if (currentShowIndex >= shows.Count)
			{
				currentShowIndex = 0;
			}

			player.Play();
        }

		public void turnOn()
        {
			Timing.RunCoroutine(turnOnSequence());

		}

		public void turnOff()
        {

			if (PlayerManager.Instance.AmyNPCLocation == PlayerNPCLocation.WatchTV ||
			PlayerManager.Instance.CreamNPCLocation == PlayerNPCLocation.WatchTV)
				return;


			Timing.RunCoroutine(turnOffSequence());
		}

		IEnumerator<float> turnOnSequence()
        {
			isOn = true;
			float fac = 0.0f;

			if (interfaceSound && onSound)
				interfaceSound.PlayOneShot(onSound);

			lookTarget.SetActive(true);

			while (fac < 1.0f)
            {
				sound.volume = fac * 0.25f;
				fac += Time.deltaTime * 2.0f;
				playerMat.SetColor("_EmissionColor", Color.white * fac);
				yield return 0f;
			}

			
		}

		IEnumerator<float> turnOffSequence()
        {
			isOn = false;
			float fac = 0.0f;

			if (interfaceSound && offSound)
				interfaceSound.PlayOneShot(offSound);

			lookTarget.SetActive(false);
			playerMat.SetColor("_EmissionColor", Color.white * 3.0f);


			while (fac > 0.0f)
            {
				Color clr = Color.Lerp(Color.black, Color.white * 3.0f, fac);
				fac -= (Time.deltaTime * 3.0f);

				yield return 0f;
            }


			sound.volume = 0.0f;
			playerMat.SetColor("_EmissionColor", Color.black );
		}

		void handleDebugInput()
        {

			if(Input.GetKeyDown(KeyCode.F10))
            {
				player.time = player.clip.length * 0.98f;
            }
        }
	}
}
