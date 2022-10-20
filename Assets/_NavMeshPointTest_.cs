using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class _NavMeshPointTest_ : MonoBehaviour
    {


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void LateUpdate()
        {
            if (!PlayerManager.Instance.mPlayerInstance)
                return;

            transform.position = EnemyManager.getClosestValidDestination(transform.position, PlayerManager.Instance.mPlayerInstance.transform.position);
        }
    }
}