using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Amy
{
    public class _DistanceTest : MonoBehaviour
    {

        public float seconds = 0.0f;
        public int minutes = 3;
        public int hours = 0;

        float distance;

        public AudioClip ready;
        public AudioClip go;
        public AudioClip finish;
        AudioSource mSound;

        bool raceInProgress = false;

        // Start is called before the first frame update
        void Start()
        {
            mSound = gameObject.AddComponent<AudioSource>();
            mSound.volume = 0.5f;
            mSound.spatialBlend = 0.0f;

        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetButtonDown("Attack"))
            {
                if (!raceInProgress)
                    Timing.RunCoroutine(doCountdown());
            }
        }


        IEnumerator<float> doCountdown()
        {
            GameManager.Instance.playerInputDisabled = true;
            Player mPlayer = PlayerManager.Instance.getPlayer();

            mPlayer.GetComponent<Rigidbody>().velocity = Vector3.zero;
            mPlayer.mDesiredMovement = Vector3.zero;
            mPlayer.transform.position = Vector3.one;
            mPlayer.mDirection = Vector3.forward;

            int countTime = 3;

            while (countTime > 0)
            {
                countTime--;
                mSound.PlayOneShot(ready);
                yield return Timing.WaitForSeconds(1.0f);
            }

            mSound.PlayOneShot(go);
            GameManager.Instance.playerInputDisabled = false;

            //RACE START
            float timeLeft = seconds;
            timeLeft += minutes * 60.0f;
            timeLeft += hours * (60.0f * 60.0f);

            while(timeLeft > 0.0f)
            {
                Debug.Log("TimeLeft: " + timeLeft);
                timeLeft -= Time.deltaTime;
                yield return 0f;
            }

            mSound.PlayOneShot(finish);
            //GET DISTANCE
            Debug.Log("DISTANCE: " + Vector3.Distance(mPlayer.transform.position, Vector3.zero));

        }
    }
}