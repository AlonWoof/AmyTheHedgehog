using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class AmyHammerControl : MonoBehaviour
    {

        Transform hammerTransform;
        Transform hammerEndTransform;
        public float desiredHammerScale = 0.0f;
        public float hammerScale = 0.0f;

        bool hammerEnabled = false;

        public AudioClip pikoHitSound;
        AudioSource hammerSoundSource;

        public GameObject hammerHitFX;
        public LayerMask mColMask;

        Animator mAnimator;

        private void Awake()
        {
            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.gameObject.name.ToLower() == "weapon")
                    hammerTransform = t;
            }

            foreach (Transform t in GetComponentsInChildren<Transform>())
            {
                if (t.gameObject.name.ToLower() == "weapon_end")
                    hammerEndTransform = t;
            }

            if (!hammerTransform)
                enabled = false;

            if (!hammerSoundSource)
            {
                hammerSoundSource = gameObject.AddComponent<AudioSource>();
                hammerSoundSource.volume = 0.85f;
                hammerSoundSource.outputAudioMixerGroup = GameManager.Instance.systemData.AUDIO_GameSFXMixer.outputAudioMixerGroup;
                hammerSoundSource.spatialBlend = 0.0f;
            }

            mColMask = LayerMask.GetMask("Collision");

            mAnimator = GetComponent<Animator>();
        }


        // Start is called before the first frame update
        void Start()
        {
            if (!hammerTransform)
                return;
        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!hammerTransform)
                return;

            hammerScale = mAnimator.GetFloat("hammerScale");

            hammerTransform.localScale = Vector3.one * hammerScale;

            //hammerScale = Mathf.Lerp(hammerScale, desiredHammerScale, 0.15f);

           // if (Mathf.Abs(hammerScale - desiredHammerScale) < 0.05f)
             //   hammerScale = desiredHammerScale;
        }

        public void HammerOn()
        {
           //desiredHammerScale = 1.0f;
           // hammerScale = 0.75f;
        }

        public void HammerOff()
        {
            //desiredHammerScale = 0.0f;
        }

        public void PikoImpact()
        {

            Vector3 pos = transform.position + transform.forward * 0.65f;
            Vector3 sPos = pos + Vector3.up;
            Vector3 ePos = sPos - Vector3.up * 3;

            RaycastHit hitInfo = new RaycastHit();

            

            if(Physics.Linecast(sPos,ePos,out hitInfo, mColMask))
            {
                pos = hitInfo.point;
            }


            GameObject inst = GameObject.Instantiate(hammerHitFX);
            inst.transform.position = pos;



            

            
        }
    }
}