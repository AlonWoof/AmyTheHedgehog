using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class StealthCamo : MonoBehaviour
    {
        const float maxTime = 30.0f;

        //30 seconds of invisibility til it breaks
        float timeLeft = 30.0f;

        List<Material> mats;
        List<Shader> originalShaders;

        Player mPlayer;

        float veloFactor = 0.0f;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            timeLeft = 30.0f;

            PlayerManager.Instance.playerHasStealthCamo = true;
            mPlayer = GetComponent<Player>();

            mats = new List<Material>();
            originalShaders = new List<Shader>();

            foreach (Renderer r in GetComponentsInChildren<Renderer>())
            {
                foreach (Material m in r.materials)
                {
                    mats.Add(m);
                    originalShaders.Add(m.shader);
                }
            }

            foreach (Material m in mats)
            {
                m.shader = Shader.Find("StealthCamo");
            }
        }

        void animateShaders()
        {
            if (!mPlayer)
                return;

            veloFactor = Mathf.Lerp(veloFactor, (mPlayer.mForwardVelocity/4.0f), Time.deltaTime);

            float distort = 0.1f * veloFactor;

            if (timeLeft < 5.0f)
            {

                if (Time.frameCount % 16 == 0)
                    veloFactor = Random.Range(-4, 4);


                if (timeLeft < 3.0f)
                {
                    if (Time.frameCount % 8 == 0)
                        veloFactor = Random.Range(-16,16);
                }
            }

            foreach (Material m in mats)
            {
                m.SetFloat("_Distortion", distort);
            }
        }

        private void OnDisable()
        {
            PlayerManager.Instance.playerHasStealthCamo = false;

            for (int i = 0; i < mats.Count; i++)
            {
                mats[i].shader = originalShaders[i];
            }
        }

        // Update is called once per frame
        void Update()
        {
            timeLeft -= Time.deltaTime;

            animateShaders();

            if (timeLeft <= 0.0f)
                enabled = false;
        }
    }
}