using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class SwitchCharacter : MonoBehaviour
    {
        public PlayableCharacter switchToCharacter;

        public void doSwitch()
        {
            PlayerManager.Instance.characterSwitch(switchToCharacter);
        }
    }
}