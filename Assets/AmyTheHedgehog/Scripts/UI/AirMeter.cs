using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amy
{

    public class AirMeter : MonoBehaviour
    {
        public Image airMeter_circle;
        public CanvasGroup mCanvas;
        Player mPlayer;

        public Gradient colorGradient;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(!mPlayer)
            {
                mPlayer = PlayerManager.Instance.getPlayer();
                return;
            }


            if(mPlayer.airLeft >= mPlayer.lungCapacity)
            {
                mCanvas.alpha = Mathf.Lerp(mCanvas.alpha, 0.0f, Time.deltaTime * 16.0f);
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 2.0f, Time.deltaTime * 8.0f);
            }
            else
            {
                mCanvas.alpha = Mathf.Lerp(mCanvas.alpha, 1.0f, Time.deltaTime * 8.0f);
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one, Time.deltaTime * 16.0f);
            }

            float airFac = mPlayer.airLeft / mPlayer.lungCapacity;

            airMeter_circle.fillAmount = airFac;
            airMeter_circle.color = colorGradient.Evaluate(airFac);
        }


    }
}