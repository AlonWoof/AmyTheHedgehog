using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amy
{

    

    public class HealthMeter : MonoBehaviour
    {

        public Image healthBar;
        public Image healthBarBG;
        public Image staminaBar;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            PlayerStatus pstats = PlayerManager.Instance.getCurrentPlayerStatus();

            float targetScale = (pstats.currentHealth / pstats.maxHealth);

            healthBar.transform.localScale = new Vector3(1, targetScale, 1);
            healthBarBG.transform.localScale = Vector3.Lerp(healthBarBG.transform.localScale, new Vector3(1, targetScale, 1), Time.deltaTime * 4.0f);

            targetScale = (pstats.currentMagic / pstats.maxMagic);

            staminaBar.transform.localScale = new Vector3(1, targetScale, 1);
        }
    }
}