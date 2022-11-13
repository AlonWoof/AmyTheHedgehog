using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class PlayerActivator : PlayerMode
    {

        public Activatible closestActivatible;
        List<Activatible> currentActivatibles;

        const float maxActivatibleDist = 32.0f;
        const float tickTime = 0.5f;

        float tLeft = 0.5f;
        float activateTimeout = 1.0f;

        // Start is called before the first frame update
        void Start()
        {
            currentActivatibles = new List<Activatible>();
            getBaseComponents();
        }

        // Update is called once per frame
        void Update()
        {
            if(tLeft > 0.0f)
            {
                tLeft -= Time.deltaTime;
            }
            else
            {
                tLeft = tickTime;
                scanForActivatibles();
            }

            if (activateTimeout > 0.0f)
                activateTimeout -= Time.deltaTime;


            if (GameManager.Instance.playerInputDisabled)
                return;

            if(Input.GetButtonDown("Action"))
            {
                Activate();
            }
        }

        void scanForActivatibles()
        {
            currentActivatibles.Clear();
            closestActivatible = null;

            float bestDist = 64.0f;

            foreach (Activatible a in FindObjectsOfType<Activatible>())
            {
                Vector3 mPos = transform.position + Vector3.up * (mPlayer.playerHeight * 0.85f);
                Vector3 aPos = a.transform.position + a.offset;

                float dist = Vector3.Distance(mPos, aPos);
                
                if(dist < maxActivatibleDist)
                {
                    currentActivatibles.Add(a);

                    if(dist < bestDist && dist < a.range)
                    {

                        bestDist = dist;
                        closestActivatible = a;
                    }
                }
            }
        }


        void Activate()
        {
            if (closestActivatible == null)
                return;

            if (activateTimeout > 0.0f)
                return;

            Vector3 apos = closestActivatible.transform.position;
            Vector3 mpos = transform.position;

            mpos.y = 0;
            apos.y = 0;

            
            Vector3 directionTo = Helper.getDirectionTo(mpos, apos);

            Debug.Log("DOT PRODUCT: " + Vector3.Dot(transform.forward, directionTo));

            if (Vector3.Dot(transform.forward, directionTo) < 0.65f)
                return;

            if(closestActivatible.directionDependent)
            {
                if (Vector3.Dot(transform.forward, closestActivatible.transform.forward) > -0.5f)
                    return;
            }

            closestActivatible.onActivate.Invoke();
            activateTimeout = 1.0f;
        }
    }
}