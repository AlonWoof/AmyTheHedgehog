using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

//////////////////////////////////////
//         2023 AlonWoof            //
//////////////////////////////////////

namespace Amy
{
	[System.Serializable]
	public class HitboxData
    {
		public string boneName = "root";

		public enum HitboxShape
        {
			SPHERE,
			CAPSULE,
			BOX
        }

		public HitboxShape mShape;
		public DamageType Weakness;
		public DamageType Immunity;

		public float damageMultiplier = 1.0f;

		
		public float radius = 0.0f;
		public float height = 0.0f;
		public Vector3 center = Vector3.zero;
		public Vector3 size = Vector3.zero;

		public Hitbox addToTransform(Transform t)
        {

			if (!t)
				return null;

			GameObject hbox = new GameObject("hitbox_" + t.name);
			hbox.transform.SetParent(t);
			hbox.transform.localPosition = Vector3.zero;
			hbox.transform.localRotation = Quaternion.Euler(Vector3.zero);

			Hitbox hitBox = hbox.AddComponent<Hitbox>();
			hitBox.damageMultiplier = damageMultiplier;
			hitBox.Weakness = Weakness;
			hitBox.Immunity = Immunity;

			Collider col = null;

			switch(mShape)
            {
				case HitboxShape.BOX:
					BoxCollider box = hbox.AddComponent<BoxCollider>();
					box.center = center;
					box.size = size;
					col = box;
					break;
				case HitboxShape.CAPSULE:
					CapsuleCollider capsule = hbox.AddComponent<CapsuleCollider>();
					capsule.center = center;
					capsule.radius = radius;
					capsule.height = height;
					col = capsule;
					break;
				case HitboxShape.SPHERE:
					SphereCollider sphere = hbox.AddComponent<SphereCollider>();
					sphere.radius = radius;
					sphere.center = center;
					col = sphere;
					break;
            }

			if (col == null)
				return null;

			col.isTrigger = true;

			return hitBox;
        }
    }

	public class Hitbox : MonoBehaviour
	{

		public DamageType Weakness;
		public DamageType Immunity;
		public DamageTeam damageTeam;

		public float damageMultiplier = 1.0f;
		public bool isPlayerHitbox = false;

		public Player mPlayer;
		public Enemy mEnemy;

        public void Update()
        {
            if(!mPlayer && !mEnemy)
            {
				mPlayer = GetComponent<Player>();
				mEnemy = GetComponent<Enemy>();
            }

			if (damageTeam == DamageTeam.Player)
				transform.gameObject.layer = LayerMask.NameToLayer("PlayerHitbox");
			else
				transform.gameObject.layer = LayerMask.NameToLayer("EnemyHitbox");
		}

        public void onTakeDamage(Damage dmg)
        {
			float mult = damageMultiplier;

			if(mPlayer != null)
            {
				//She'll obtain an ouchies

				mPlayer.takeDamage(dmg, mult);
            }

			if(mEnemy != null)
            {
				//Enemy oofer

				mEnemy.takeDamage(dmg);
            }
        }

	}
}
