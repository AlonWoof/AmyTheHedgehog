using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amy
{

    

    public class HealthMeter : MonoBehaviour
    {

        public Image healthBar;
        public Image moodBar;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            PlayerStatus pstats = PlayerManager.Instance.getCurrentPlayerStatus();

            float targetScale = (pstats.currentHealth / pstats.maxHealth);

            healthBar.transform.localScale = new Vector3(targetScale, 1, 1);

            targetScale = (pstats.currentMood / pstats.maxMood);

            moodBar.transform.localScale = new Vector3(targetScale, 1, 1);
        }
    }
}