using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class PlayerSwimming : PlayerMode
    {

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            if (mPlayer.currentMode != PlayerModes.SWIMMING)
                return;


        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
