using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MEC;

namespace Amy
{
    public class Robot : MonoBehaviour
    {

        public enum HearingResult { None, Quiet, Loud };
        public enum VisionResult { None, Far, Close };

        public List<Waypoint> homePatrolRoute;
        public float searchRadius = 32.0f;

        NavMeshAgent mAgent;
        Animator mAnimator;

        public float sightRangeNormal = 32.0f;

        public float sightFOVAngle = 45.0f;

        public float hearingRange = 6.0f;

        public Transform eye;

        LayerMask visionMask;

        Vector3 mDirection;
        float velocity = 0.0f;

        public bool hasVisualOnPlayer = false;

        public CoroutineHandle currentMode;
        public CoroutineHandle subAction;

        public bool isDead = false;
        public bool isSearching = false;
        public bool isAlertMode = false;


        public VisionResult lastVisionCheck;

        private void Awake()
        {
            visionMask = LayerMask.GetMask("PlayerHitbox", "Collision");
            mAgent = GetComponent<NavMeshAgent>();

            mAgent.updateRotation = false;
            mDirection = transform.forward;
            mAnimator = GetComponent<Animator>();

            foreach (Transform t in gameObject.GetComponentsInChildren<Transform>())
            {
                if(t.gameObject.name.ToLower() == "head")
                {
                    GameObject inst = new GameObject("Eye");
                    inst.transform.SetParent(t);

                    inst.transform.position = t.transform.position + transform.forward * 0.2f + Vector3.up * 0.1f;

                    eye = inst.transform;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {
            PatrolMode();
        }

        public float calculateSightRange()
        {
            float ret = sightRangeNormal;

            //20% bigger while actively looking for Amy
            if (EnemyManager.Instance.currentEnemyPhase == ENEMY_PHASE.PHASE_ALERT)
                ret *= 1.2f;

            //If you got, say, stealth camo.... or crouching, or in tall grass...
            ret *= (1.0f - PlayerManager.Instance.stealthIndex);

            return ret;
        }

        // Update is called once per frame
        void Update()
        {

            lastVisionCheck = visionCheck();

            velocity = mAgent.velocity.magnitude;

            mAnimator.SetFloat("velocity", velocity/2.0f);

            if (Vector3.Distance(transform.position, mAgent.destination) < 0.5f)
                mAgent.enabled = false;


            if (velocity > 0.1f && mAgent.enabled)
            {
                mDirection = mAgent.velocity.normalized;
            }
            else
            {
                if (hasVisualOnPlayer)
                {
                    mDirection = Helper.getDirectionTo(transform.position, EnemyManager.Instance.lastPlayerLocation);

                    float angle = -Vector3.Angle(eye.forward, Helper.getDirectionTo(eye.position, PlayerManager.Instance.mPlayerInstance.transform.position + Vector3.up * 0.5f));

                    mAnimator.SetFloat("aimAngle", angle/45.0f);
                }
            }


        }

        public void killModeCoroutines()
        {

            if (currentMode.IsRunning)
                Timing.KillCoroutines(currentMode);

            if (subAction.IsRunning)
                Timing.KillCoroutines(subAction);
        }

        public void Exclamation()
        {

            //Metal Gear exclamation point moment
            killModeCoroutines();
            Timing.RunCoroutine(doExclamation());
        }

        public IEnumerator<float> doExclamation()
        {
            
            GameObject fx = GameObject.Instantiate(GameManager.Instance.systemData.RES_AI_ExclamationFX);
            fx.transform.position = eye.transform.position;


            mDirection = Helper.getDirectionTo(transform.position, EnemyManager.Instance.lastPlayerLocation);

            yield return Timing.WaitForSeconds(1.0f);

            EnemyManager.Instance.AlertAllBotsInArea();
        }


        public void AlertMode()
        {
            killModeCoroutines();

            currentMode = Timing.RunCoroutine(Mode_Pursuit());
        }

        public void CautionMode()
        {
            killModeCoroutines();

            //currentMode = Timing.RunCoroutine(Mode_Caution());
        }

        public void EvadeMode()
        {
            killModeCoroutines();
            
            currentMode = Timing.RunCoroutine(Mode_Evasion());
        }

        public void PatrolMode()
        {
            isAlertMode = false;
            killModeCoroutines();

            currentMode = Timing.RunCoroutine(Mode_Patrol());

        }

        private void LateUpdate()
        {
            Vector3 xz_dir = mDirection;

            xz_dir.y = 0;
            xz_dir = xz_dir.normalized;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(xz_dir), Time.deltaTime * 6.0f);

            Debug.DrawLine(eye.position, eye.position + eye.forward * sightRangeNormal);

            snapToGround();
        }


        public IEnumerator<float> Mode_Patrol()
        {
            //Just move from waypoint to waypoint, sometimes stopping to look around.

            Debug.Log("Current mode - Patrol");

            int iterator = 0;
            Waypoint mWay = homePatrolRoute[0];

            float dist = Vector3.Distance(transform.position, mWay.transform.position);

            mAgent.speed = 3.0f;

            while(!isDead)
            {
                subAction = Timing.RunCoroutine(goToWaypoint(homePatrolRoute[iterator]));

                while(subAction.IsRunning && subAction.IsValid)
                {

                    if(lastVisionCheck == VisionResult.Far)
                    {
                        CoroutineHandle searchRoutine = Timing.RunCoroutine(investigateLocation(EnemyManager.Instance.lastPlayerLocation, 0.1f));

                        while(searchRoutine.IsRunning)
                        {
                            if (lastVisionCheck == VisionResult.Close)
                            {
                                Exclamation();
                            }

                            yield return 0f;
                        }
                    }
                    else if (lastVisionCheck == VisionResult.Close)
                    {
                        Exclamation();
                    }

                    yield return 0f;
                }

                float waitTime = homePatrolRoute[iterator].stopTime;

                if (homePatrolRoute[iterator].stopTime > 0.0f)
                {
                    if (homePatrolRoute[iterator].stopTime > 4.0f)
                        mAnimator.Play("LookAround");

                    while (waitTime > 0.0f)
                    { 

                        //mAgent.enabled = false;

                        if (lastVisionCheck == VisionResult.Far)
                        {
                            CoroutineHandle searchRoutine = Timing.RunCoroutine(investigateLocation(EnemyManager.Instance.lastPlayerLocation, 0.1f));

                            while (searchRoutine.IsRunning)
                            {
                                if (lastVisionCheck == VisionResult.Close)
                                {
                                    Exclamation();
                                }

                                yield return 0f;
                            }

                        }
                        else if (lastVisionCheck == VisionResult.Close)
                        {
                            Exclamation();
                        }

                        waitTime -= Time.deltaTime;

                        yield return 0f;
                    }
                }


                iterator++;

                if (iterator > homePatrolRoute.Count - 1)
                    iterator = 0;

                yield return 0f;
            }

            yield return 0f;
        }

        public IEnumerator<float> Mode_CheckSound(Vector3 pos)
        {
            //Huh? What was that noise?
            yield return 0f;
        }

        public IEnumerator<float> investigateLocation(Vector3 loc, float checkRange = 0.0f)
        {
            Vector3 searchPos = EnemyManager.getClosestValidDestination(transform.position,loc + new Vector3(Random.Range(-checkRange,checkRange),0, Random.Range(-checkRange, checkRange)));

            subAction = Timing.RunCoroutine(goToPosition(searchPos));

            while(subAction.IsRunning && mAgent.pathStatus != NavMeshPathStatus.PathInvalid)
            {
                //Debug.Log("Going to investigate...");
                mAgent.enabled = true;
                yield return 0f;
            }

            mAgent.enabled = false;
            mAnimator.Play("LookAround");
           // Debug.Log("Lookin' Around...");

            yield return Timing.WaitForSeconds(4.0f);
        }



        public IEnumerator<float> Mode_Pursuit()
        {

            mAgent.speed = 6.0f;

            float attackRange = Random.Range(8, 10);
            bool losingTarget = false;

            int noVisionFrames = 0;

            Debug.Log("Current mode - Pursuit");

            while (!isDead)
            {

                float dist = Vector3.Distance(transform.position, EnemyManager.Instance.playerActualLocation);


                if (dist > attackRange && !losingTarget)
                    losingTarget = true;

                if (dist < attackRange * 0.75f)
                    losingTarget = false;

                if(losingTarget)
                {
                    mAgent.enabled = true;
                    mAgent.SetDestination(EnemyManager.getClosestValidDestination(transform.position, EnemyManager.Instance.lastPlayerLocation));
                }
                else
                {
                    mAgent.enabled = false;
                }




                if (lastVisionCheck != VisionResult.None)
                {
                    EnemyManager.Instance.resetAlertTimer();
                    mAnimator.SetBool("isAiming", true);

                    noVisionFrames = 0;
                }
                else
                {
                    noVisionFrames++;

                    if (noVisionFrames > 30)
                    {
                        mAnimator.SetBool("isAiming", false);
                    }
                }


                yield return 0f;
            }

        }

        public IEnumerator<float> Mode_Evasion()
        {
            Debug.Log("Current mode - Evade");

            while (!isDead)
            {
                CoroutineHandle searchAction = Timing.RunCoroutine(investigateLocation(EnemyManager.Instance.lastPlayerLocation, 5.0f));

                while (searchAction.IsRunning)
                {

                    if (lastVisionCheck == VisionResult.Close || lastVisionCheck == VisionResult.Far)
                    {
                        Exclamation();
                    }

                    yield return 0f;
                }

                yield return 0f;
            }
        }


        public IEnumerator<float> goToWaypoint(Waypoint w)
        {
            Vector3 waypointPos = EnemyManager.getClosestValidDestination(transform.position, w.transform.position);

            float dist = Vector3.Distance(transform.position, waypointPos);

            while(dist > 1.0f)
            {
                mAgent.enabled = true;
                dist = Vector3.Distance(transform.position, waypointPos);

                mAgent.SetDestination(waypointPos);

                yield return 0f;
            }
        }

        public IEnumerator<float> goToPosition(Vector3 pos)
        {
           
            Vector3 dest = EnemyManager.getClosestValidDestination(transform.position, pos);

            float dist = Vector3.Distance(transform.position, dest);

            while (dist > 1.0f)
            {
                mAgent.enabled = true;
                dist = Vector3.Distance(transform.position, dest);

                mAgent.SetDestination(dest);

                if (mAgent.pathStatus == NavMeshPathStatus.PathInvalid)
                    dist = 0.0f;

                yield return 0f;
            }

        }

        void snapToGround()
        {
            LayerMask col = LayerMask.GetMask("Collision");

            Vector3 start = transform.position + Vector3.up;
            Vector3 end = transform.position - Vector3.up * 0.25f;

            RaycastHit hitInfo = new RaycastHit();

            if(Physics.Linecast(start,end,out hitInfo,col))
            {
                transform.position = hitInfo.point;
            }
        }

        HearingResult hearingCheck()
        {
            foreach (SoundMarker s in FindObjectsOfType<SoundMarker>())
            {
                float dist = Vector3.Distance(eye.transform.position, s.transform.position);

                if (dist > hearingRange + s.soundRange)
                {
                    if (s.soundRange > 32.0f)
                        return HearingResult.Loud;

                    return HearingResult.Quiet;
                }
            }

            return HearingResult.None;
        }

        VisionResult visionCheck()
        {
            //Can't see the player if they don't exist.
            if (!PlayerManager.Instance.mPlayerInstance)
                return VisionResult.None;

            hasVisualOnPlayer = false;

            Vector3 los_start = eye.position;
            Vector3 los_end = PlayerManager.Instance.mPlayerInstance.transform.position + Vector3.up * Random.Range(0.25f, 0.75f);

            RaycastHit hitInfo = new RaycastHit();

            if (Vector3.Angle(eye.forward, Helper.getDirectionTo(eye.position, los_end)) > sightFOVAngle)
                return VisionResult.None;

            float range = calculateSightRange();

            if (Physics.Linecast(los_start,los_end, out hitInfo, visionMask))
            {
                float dist = Vector3.Distance(los_start, hitInfo.point);

                if (hitInfo.collider.GetComponentInChildren<PlayerHitbox>())
                {


                    if (dist < range)
                    {
                        EnemyManager.Instance.lastPlayerLocation = hitInfo.point;
                        

                        if (dist > range * 0.85f)
                        {
                            Debug.DrawLine(los_start, los_end, Color.yellow, 0.2f);
                            return VisionResult.Far;
                        }

                        hasVisualOnPlayer = true;

                        Debug.DrawLine(los_start, los_end, Color.red, 0.2f);
                        return VisionResult.Close;

                    }

                }
            }

            Debug.DrawLine(los_start, los_end, Color.green);

            return VisionResult.None;
        }
    }
}