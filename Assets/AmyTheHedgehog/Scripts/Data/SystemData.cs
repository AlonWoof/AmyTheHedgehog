using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    [System.Serializable]
    [CreateAssetMenu(fileName = "SystemData", menuName = "SystemData", order = 51)]
    public class SystemData : ScriptableObject
    {
        public GameObject RES_AmyIngameModel;
        public RuntimeAnimatorController RES_AmyIngameAnimator;

        public GameObject RES_mainCamera;
    }
}