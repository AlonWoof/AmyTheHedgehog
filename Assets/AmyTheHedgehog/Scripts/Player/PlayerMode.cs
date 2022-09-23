using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RootMotion.FinalIK;

/* Copyright 2022 Jason Haden */
namespace Amy
{

	public abstract class PlayerMode : MonoBehaviour
	{

        public Player mPlayer;
        public CapsuleCollider mCollider;
        public Rigidbody mRigidBody;
        public Animator mAnimator;

        public GrounderFBBIK grounderIK;
        

        public void getBaseComponents()
        {
            mPlayer = GetComponent<Player>();
            mCollider = GetComponent<CapsuleCollider>();
            mRigidBody = GetComponent<Rigidbody>();
            mAnimator = GetComponent<Animator>();
            grounderIK = GetComponent<GrounderFBBIK>();
        }

        public void onStartMode()
        {

        }

        public void onEndMode()
        {

        }

        

	}

}
