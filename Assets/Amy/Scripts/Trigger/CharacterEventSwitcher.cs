using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Amy
{
    /// <summary>
    /// Enables/disables gameobjects based on current character.
    /// </summary>

    public class CharacterEventSwitcher : MonoBehaviour
    {

        public List<GameObject> amy_objects;
        public List<GameObject> cream_objects;

        public UnityEvent amyEvents;
        public UnityEvent creamEvents;

        // Start is called before the first frame update
        void Start()
        {
            switchAllObjects();
        }

        // Update is called once per frame
        void Update()
        {
            //switchAllObjects();
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

                    amyEvents.Invoke();
                    break;

                case PlayableCharacter.Cream:
                    foreach (GameObject g in cream_objects)
                        g.SetActive(true);

                    creamEvents.Invoke();
                    break;
            }
        }
    }
}