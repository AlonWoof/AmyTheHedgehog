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

            float targetScale = PlayerManager.Instance.getCurrentPlayerStatus().currentHealth;

            healthBar.transform.localScale = new Vector3(targetScale, 1, 1);

            targetScale = PlayerManager.Instance.getCurrentPlayerStatus().currentMood;

            moodBar.transform.localScale = new Vector3(targetScale, 1, 1);
        }
    }
}