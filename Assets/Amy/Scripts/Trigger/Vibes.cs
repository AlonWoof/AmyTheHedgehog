using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
    [System.Flags]
    public enum VibeType
    {
        Neutral = 0,
        Peaceful = 1,
        Pretty = 2,
        Fun = 4,
        Dark = 8,
        Scary = 16,
        Dirty = 32
    }

	public class Vibes : MonoBehaviour
	{
		public float range = 16.0f;


        public VibeType vibeFlags;

        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, range);
        }

        void OnGUI()
        {
            
        }

    }
}
