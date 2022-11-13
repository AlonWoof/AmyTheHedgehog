using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    /// <summary>
    /// Enables/disables gameobjects based on current character.
    /// </summary>

    public class CharacterEventSwitcher : MonoBehaviour
    {

        public List<GameObject> amy_objects;
        public List<GameObject> cream_objects;

        // Start is called before the first frame update
        void Start()
        {
            switchAllObjects();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void switchAllObjects()
        {
            foreach (GameObject g in amy_objects)
                g.SetActive(false);


            foreach (GameObject g in cream_objects)
                g.SetActive(false);


            switch (PlayerManager.Instance.currentCharacter)
            {
                case PlayableCharacter.Amy:
                    foreach (GameObject g in amy_objects)
                        g.SetActive(true);
                    break;

                case PlayableCharacter.Cream:
                    foreach (GameObject g in cream_objects)
                        g.SetActive(true);
                    break;
            }
        }
    }
}