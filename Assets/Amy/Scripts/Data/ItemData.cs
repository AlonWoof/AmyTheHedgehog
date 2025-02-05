using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	[System.Serializable]
	[CreateAssetMenu(fileName = "New ItemData", menuName = "ItemData", order = 51)]
	public class ItemData : ScriptableObject
	{
		public int hash;
		public string displayName;
		[TextArea]
		public string description;
		public Sprite icon;
		public bool consumable = true;


		public GameObject itemEffect = null;

		public virtual void useItem()
        {
			Timing.RunCoroutine(doUseItem());
		}

		public IEnumerator<float> doUseItem()
        {
			yield return Timing.WaitForSeconds(0.2f);

			if (!PlayerManager.Instance.mPlayerInstance)
				yield break;

			if (!itemEffect)
				yield break;

			GameObject inst = GameObject.Instantiate(itemEffect);
			inst.transform.position = PlayerManager.Instance.mPlayerInstance.transform.position + Vector3.up * 0.5f;
			inst.transform.rotation = PlayerManager.Instance.mPlayerInstance.transform.rotation;

			
		}

		public int getHash()
        {
			if (name == null)
				return 0;

			return Animator.StringToHash(name.ToLower());
        }

        private void OnValidate()
        {
			hash = getHash();

		}

        public static ItemData getItemData(string name)
        {
			return getItemData(Animator.StringToHash(name.ToLower()));
        }

		public static ItemData getItemData(int hash)
        {

			foreach (ItemData i in GameManager.Instance.systemData.itemData)
			{
				if (Animator.StringToHash(i.name.ToLower()) == hash)
				{
					return i;
				}
			}

			return null;
        }
	}
}
