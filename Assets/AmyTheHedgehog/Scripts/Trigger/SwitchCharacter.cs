using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class SwitchCharacter : MonoBehaviour
    {
        public PlayableCharacter switchToCharacter;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void doSwitch()
        {

            PlayerManager.Instance.characterSwitch(switchToCharacter);
        }
    }
}