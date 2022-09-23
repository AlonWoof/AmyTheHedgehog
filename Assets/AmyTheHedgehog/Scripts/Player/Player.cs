using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Amy
{

	public class Player : MonoBehaviour
	{


        const float rotationSpeed = 10.0f;

        Animator mAnimator;
        CapsuleCollider mCollider;
        Rigidbody mRigidBody;

        //This vector must always be normalized.
        public Vector3 mDirection = Vector3.forward;

        public Vector3 mCurrentMovement = Vector3.zero;
        public Vector3 mDesiredMovement = Vector3.zero;

        public Vector3 groundNormal;
        public bool isGrounded;

        public LayerMask mColMask;

        public float turnRatio = 0.0f;
        public float stealthIndex = 0.0f;

        //Modes
        PlayerGroundMove modeGroundMove;

        private void Awake()
        {
            mColMask = LayerMask.GetMask("Collision");

        }

        public static Player Spawn(Vector3 pos, Vector3 dir)
        {
            float amy_height = 0.9f;

            //Fandom wiki says 25 kg, official sources cheekily say ヒ・ミ・ツ！ ("it's a secret!")
            //So let's assume 25kg, seems about right.
            float amy_weight = 25.0f;

            GameObject inst = GameObject.Instantiate(GameManager.Instance.systemData.RES_AmyIngameModel);
            inst.transform.position = pos;


            CapsuleCollider col = inst.AddComponent<CapsuleCollider>();
            col.radius = 0.25f;
            col.height = amy_height * 0.85f;
            col.center = (Vector3.up * amy_height) * 0.5f;

            Rigidbody body = inst.AddComponent<Rigidbody>();
            body.mass = amy_weight;

            body.useGravity = false;
            body.freezeRotation = true;
            body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

            //Animator anim = inst.AddComponent<Animator>();
            //anim.runtimeAnimatorController = GameManager.Instance.systemData.RES_AmyIngameAnimator;

            Player newPlayer = inst.AddComponent<Player>();

            GameObject camInst = new GameObject("ThirdPersonCamera");
            ThirdPersonCamera tpc = camInst.AddComponent<ThirdPersonCamera>();

            tpc.setPlayerTransform(inst.transform);

            
            newPlayer.mDirection = dir;

            return newPlayer;
        }

        void getBaseComponents()
        {
            mAnimator = GetComponent<Animator>();
            mCollider = GetComponent<CapsuleCollider>();
            mRigidBody = GetComponent<Rigidbody>();

            //Add a dummy animator to prevent problems later.
            if (!mAnimator)
                mAnimator = gameObject.AddComponent<Animator>();
        }

        void addAllModes()
        {
            modeGroundMove = gameObject.AddComponent<PlayerGroundMove>();

        }

    	// Start is called before the first frame update
    	void Start()
    	{
            getBaseComponents();
            addAllModes();
        }

    	// Update is called once per frame
    	void Update()
    	{
            Debug.DrawLine(transform.position, transform.position + groundNormal, Color.red, 1.0f);
        }


        void updateDirection()
        {

            transform.rotation = (Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(mDirection, Vector3.up), Time.fixedDeltaTime * rotationSpeed));
            turnRatio = (1.0f - Vector3.Dot(transform.forward, mDirection));

        }


        private void FixedUpdate()
        {
            updateDirection();
        }
    }

}
