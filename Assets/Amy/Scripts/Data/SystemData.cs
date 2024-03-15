using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Amy
{

    [System.Serializable]
    public class AmyFXRes
    {
        public GameObject fx_basicJump;
        public GameObject fx_pikoHammerHit;
        public GameObject fx_pikoHammerJump;
        public GameObject fx_pikoHammerTrail;
        public GameObject prop_amyCloth;
        public GameObject prop_creamCloth;

        public GameObject basicSlingshotProjectile;
    }

    [System.Serializable]
    [CreateAssetMenu(fileName = "SystemData", menuName = "SystemData", order = 51)]
    public class SystemData : ScriptableObject
    {

        public PlayerParameters AmyParams;
        public PlayerParameters CreamParams;

        public GameObject RES_userInterface;
        public GameObject RES_mainCamera;
        public AmyFXRes RES_AmyPlayerFX;
        public FootstepFXRes RES_footstepFX;

        public GameObject RES_AI_ExclamationFX;

        public GameObject RES_WaterWadingFX;
        public GameObject RES_ActorWaterSplashFX;
        public GameObject RES_ActorWaterEmergeFX;
        public GameObject RES_RingTobitiri;

        [Header("Audio")]

        public AudioMixer AUDIO_GameSFXMixer;
        public AudioMixer AUDIO_MenuSFXMixer;
        public AudioMixer AUDIO_MusicMixer;
        public AudioMixerGroup AUDIO_Group_Voice;

        public BGMData bgm_alert;
        public BGMData bgm_evasion;
        public AudioClip sfx_mgs_clear;
    }
}