using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//////////////////////////////////////
//         2024 AlonWoof            //
//////////////////////////////////////

namespace Amy
{

	public enum CharaTag
	{
		None,
		Amy,
		Cream,
		Yume
    }


	public class CharacterTags : MonoBehaviour
	{

		public CharaTag character;

		public static void disableCharacterByTag(CharaTag tag)
        {
			foreach(CharacterTags t in FindObjectsOfType<CharacterTags>())
            {
				if(t.character == tag)
                {
					t.gameObject.SetActive(false);
                }
            }
        }
	}
}
