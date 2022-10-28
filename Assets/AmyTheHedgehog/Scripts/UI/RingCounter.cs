using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Amy
{
    public class RingCounter : MonoBehaviour
    {
        public Text mText;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

            mText.text = PlayerManager.Instance.ringCount.ToString();
        }


    }
}
