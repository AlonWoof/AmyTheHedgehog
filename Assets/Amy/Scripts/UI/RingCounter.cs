using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amy
{
    public class RingCounter : MonoBehaviour
    {
        public Text ringText;
        public Text bankText;

        public GameObject ringCounter;
        public GameObject bankCounter;

        // Start is called before the first frame update
        void Start()
        {
            bankCounter.SetActive(false);
            ringCounter.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            bankText.text = PlayerManager.Instance.getTotalRings().ToString("00000");
            ringText.text = PlayerManager.Instance.getRings().ToString("000");

            if(!ringCounter.activeInHierarchy && !PlayerManager.Instance.isHubWorld)
            {
                ringCounter.SetActive(true);
                bankCounter.SetActive(false);
            }
            else if(ringCounter.activeInHierarchy  && PlayerManager.Instance.isHubWorld)
            {
                ringCounter.SetActive(false);
                bankCounter.SetActive(true);
            }


        }


    }
}
