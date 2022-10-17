using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

namespace Amy
{

    public class MusicManager : Singleton<MusicManager>
    {
        public BGMData currentBGM = null;
        public AudioSource bgm;

        public AudioSource combatBGM;

        bool hasOutro = false;
        bool hasIntro = false;

        bool isFading = false;

        const float bgm_volume = 1.0f;



        public void Init()
        {

        }

        private void Awake()
        {
            bgm = gameObject.AddComponent<AudioSource>();
            bgm.volume = bgm_volume;
            bgm.spatialBlend = 0.0f;
            bgm.bypassEffects = true;
            bgm.dopplerLevel = 0.0f;

            bgm.outputAudioMixerGroup = GameManager.Instance.systemData.AUDIO_MenuSFXMixer.FindMatchingGroups("BGM")[0];

            combatBGM = gameObject.AddComponent<AudioSource>();
            combatBGM.volume = bgm_volume;
            combatBGM.spatialBlend = 0.0f;
            combatBGM.bypassEffects = true;
            combatBGM.dopplerLevel = 0.0f;

            combatBGM.outputAudioMixerGroup = GameManager.Instance.systemData.AUDIO_MenuSFXMixer.FindMatchingGroups("BGM")[0];

            // combatBGM.clip = GameManager.Instance.systemData.combatBGM;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
           // Debug.Log("BGM VOLUME: " + bgm.volume);

            if(hasIntro && bgm.clip == currentBGM.introClip && !bgm.isPlaying)
            {
                //continue to the main event.
                bgm.clip = currentBGM.mainClip;
                bgm.loop = true;
                bgm.Play();
            }
        }

        public void changeSongs(BGMData newSong, float crossFadeTime = 1.0f)
        {

            if (newSong == null)
            {
                currentBGM = null;

                //Make musicless levels fade to silence instead of continuing the last track.
                fadeBGM(0.0f, crossFadeTime);
                return;
            }

            if (currentBGM != null)
            {
                if (currentBGM.songNameHash == newSong.songNameHash)
                    return;
            }


            currentBGM = newSong;

            hasIntro = false;
            hasOutro = false;

            if (currentBGM.introClip != null)
                hasIntro = true;

            if (currentBGM.outroClip != null)
                hasOutro = true;

            Timing.RunCoroutine(doSongTransition(crossFadeTime));
        }

        IEnumerator<float> doSongTransition(float crossFadeTime)
        {


            fadeBGM(0.0f, crossFadeTime);

            while (isFading)
                yield return 0f;

            bgm.volume = bgm_volume * currentBGM.volumeMult;

            if (hasIntro)
            {
                bgm.clip = currentBGM.introClip;
                bgm.loop = false;
            }
            else
            {
                bgm.clip = currentBGM.mainClip;
                bgm.loop = true;
            }

            bgm.Play();

        }

        public void fadeBGM(float targetVolume = 0.0f, float crossFadeTime = 1.0f)
        {
            Timing.RunCoroutine(doBGMFade(targetVolume, crossFadeTime));
        }

        public void fadeCombatBGM(float targetVolume = 0.0f, float crossFadeTime = 1.0f)
        {
            Timing.RunCoroutine(doCombatBGMFade(targetVolume, crossFadeTime));
        }

        IEnumerator<float> doBGMFade(float target, float crossFadeTime)
        {

            while (isFading)
                yield return 0f;

            float totalTime = crossFadeTime;
            float timeLeft = totalTime;
            float vol = bgm.volume;


            isFading = true;
            while (timeLeft > 0)
            {
                float fac = (timeLeft / totalTime);

                bgm.volume = Mathf.Lerp(target, vol,  fac);

               // Debug.Log("BGM VOLUME: " + bgm.volume);

                timeLeft -= Time.deltaTime;
                yield return 0f;
            }

            bgm.volume = target;

            isFading = false;
        }

        IEnumerator<float> doCombatBGMFade(float target, float crossFadeTime)
        {
            while (isFading)
                yield return 0f;


            float totalTime = crossFadeTime;
            float timeLeft = totalTime;
            float vol = bgm.volume;

            isFading = true;

            while (timeLeft > 0)
            {
                float fac = (timeLeft / totalTime);

               combatBGM.volume = Mathf.Lerp(target, vol, fac);

                Debug.Log("BGM VOLUME: " + bgm.volume);

                timeLeft -= Time.deltaTime;
                yield return 0f;
            }

            combatBGM.volume = target;

            isFading = false;
        }

        public void playMusicEffect(AudioClip musicEffect, float fadetime = 1.0f, bool resumeAfter = false)
        {
            Timing.RunCoroutine(doMusicEffect(musicEffect, fadetime, resumeAfter));
        }

        IEnumerator<float> doMusicEffect(AudioClip musicEffect, float fadetime, bool resumeAfter)
        {
            fadeBGM(0, fadetime);

            float resumeTime = bgm.time;
            AudioClip originalClip = bgm.clip;

            while (isFading)
                yield return 0f;

            bgm.Stop();

            bgm.clip = musicEffect;
            bgm.volume = bgm_volume;
            bgm.Play();

            if(resumeAfter)
            {
                while(bgm.isPlaying)
                {
                    yield return 0f;
                }

                bgm.Stop();
                bgm.volume = 0.0f;
                bgm.clip = originalClip;
                bgm.Play();

                bgm.time = resumeTime;
                

                fadeBGM(0.5f, fadetime);
            }

        }



    }
}