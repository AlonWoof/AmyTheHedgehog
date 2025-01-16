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
        public GameObject fx_classicJump;
        public GameObject fx_pikoHammerHit;
        public GameObject fx_pikoHammerJump;
        public GameObject fx_pikoHammerTrail;

        public GameObject fx_cunnyDrip;

        public GameObject fx_amyMagicCircle;
        public GameObject fx_amyWarpIn;
        public GameObject fx_creamMagicCircle;
        public GameObject fx_creamWarpIn;

        public GameObject prop_amyCloth;
        public GameObject prop_creamCloth;

        public GameObject fx_creamEarFlap;
        public GameObject fx_creamButtSlamAura;
        public GameObject fx_creamButtSlamImpact;
        public GameObject fx_creamButtSlamReticule;

        public GameObject basicSlingshotProjectile;
    }


    [System.Serializable]
    [CreateAssetMenu(fileName = "SystemData", menuName = "SystemData", order = 51)]
    public class SystemData : ScriptableObject
    {

        public PlayerParameters AmyParams;
        public PlayerParameters CreamParams;
        public PlayerParameters YoungAmyParams;

        
        public GameObject RES_mainCamera;
        public GameObject RES_freeCamera;
        public AmyFXRes RES_AmyPlayerFX;
        public FootstepFXRes RES_footstepFX;

        public GameObject RES_AI_ExclamationFX;

        public GameObject RES_WaterWadingFX;
        public GameObject RES_ActorWaterSplashFX;
        public GameObject RES_ActorWaterEmergeFX;
        public GameObject RES_RingTobitiri;
        public GameObject RES_RingTobitiriFX;

        public GameObject RES_GenericHitFX;
        public GameObject RES_SpikesHitFX;

        [Header("Item")]
        public List<ItemData> itemData;

        [Header("UI")]

        public GameObject RES_userInterface;
        public GameObject RES_RingBankTransferScene;
        public GameObject RES_ReduceMoneyScene;
        public GameObject RES_NowSaving;
        public AudioClip AUDIO_pauseSound;
        public AudioClip AUDIO_confirmSound;
        public AudioClip AUDIO_selectSound;
        public AudioClip AUDIO_cancelSound;

        [Header("Audio")]

        public AudioMixer AUDIO_GameSFXMixer;
        public AudioMixer AUDIO_MenuSFXMixer;
        public AudioMixer AUDIO_MusicMixer;
        public AudioMixerGroup AUDIO_Group_Voice;

        public BGMData bgm_alert;
        public BGMData bgm_evasion;
        public AudioClip sfx_mgs_clear;
        public AudioClip AUDIO_itemGetJingle;
        public AudioClip AUDIO_challengeClearJingle;

        [Header("Cutscenes")]

        public GameObject Cutscene_AmyWakeup;
        public GameObject Cutscene_CreamWakeup;

        [Header("Message Banks")]

        public MessageBank Messages_AmyNPCDefault;
        public MessageBank Messages_CreamNPCDefault;

        public MessageBank Messages_AmyNPCShowering;
        public MessageBank Messages_CreamNPCShowering;

        public MessageBank Messages_AmyNPCWatchingTV;
        public MessageBank Messages_CreamNPCWatchingTV;
    }
}