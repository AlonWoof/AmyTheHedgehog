using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{
    public class PlayerSwimming : PlayerMode
    {

        float waterSurfacePos;
        float headOffsetFromGround;

        // Start is called before the first frame update
        void Start()
        {

        }

        private void OnEnable()
        {
            getBaseComponents();

            if (mPlayer.currentMode != PlayerModes.SWIMMING)
                return;

            headOffsetFromGround = mPlayer.headBoneTransform.position.y - transform.position.y;

            mRigidBody.velocity = Vector3.zero;

            Vector3 pos = transform.position;
            pos.y = mPlayer.getWaterYPos() - headOffsetFromGround;
            transform.position = pos;

        }

        // Update is called once per frame
        void Update()
        {

        }

        private void LateUpdate()
        {


            bindToWaterBounds();
        }

        public void bindToWaterBounds()
        {
            float y_limit = mPlayer.getWaterYPos() - headOffsetFromGround;

            if(transform.position.y > y_limit)
            {
                Vector3 npos = transform.position;
                npos.y = Mathf.Clamp(npos.y, -9000.0f, y_limit);

                transform.position = npos;
            }
        }
    }
}
