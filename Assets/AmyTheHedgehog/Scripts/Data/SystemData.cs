using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Amy
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SystemData", menuName = "SystemData", order = 51)]
    public class SystemData : ScriptableObject
    {
        public GameObject RES_AmyIngameModel;
        public RuntimeAnimatorController RES_AmyIngameAnimator;
        public CharacterPhysicsData RES_AmyJiggleData;

        public GameObject RES_userInterface;
        public GameObject RES_mainCamera;
        public AmyFXRes RES_AmyPlayerFX;
        public FootstepFXRes RES_footstepFX;


        [Header("Audio")]

        public AudioMixer AUDIO_GameSFXMixer;
        public AudioMixer AUDIO_MenuSFXMixer;
        public AudioMixer AUDIO_MusicMixer;
        public AudioMixerGroup AUDIO_Group_Voice;
    }
}