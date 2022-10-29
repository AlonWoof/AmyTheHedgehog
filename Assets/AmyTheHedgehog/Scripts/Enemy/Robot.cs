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
        RobotVoices mVoice;
        RobotGun mGun;

        //Time left 'til the next shot
        float shootTimer = 3.0f;

        //Minimum time to shoot when player is in LOS
        const float shootTime_Min = 2.0f;

        //Max time to shoot when player is in LOS
        const float shootTime_Max = 5.0f;
        

        float sightRangeNormal = 16.0f;
        float standingSightBonus = 1.2f;
        float alertSightBonus = 1.3f;

        const float sightRangeAlert = 32.0f;

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

        public bool isStationaryRobot = false;

        private void Awake()
        {
            visionMask = LayerMask.GetMask("PlayerHitbox", "EnemyHitbox", "Collision", "EnemySightBlocker");
            mAgent = GetComponent<NavMeshAgent>();

            if(mAgent)
                mAgent.updateRotation = false;

            mDirection = transform.forward;
            mAnimator = GetComponent<Animator>();
            mGun = GetComponent<RobotGun>();
            mVoice = GetComponent<RobotVoices>();

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

            //Can see further when standing still.
            if (velocity < 0.1f)
                ret *= standingSightBonus;

            //If you got, say, stealth camo.... or crouching, or in tall grass...
            ret *= (1.0f - PlayerManager.Instance.stealthIndex);

            return ret;
        }

        private void OnDrawGizmos()
        {
            if (Application.isEditor)
                return;

            Gizmos.color = new Color(1, 0.65f, 0.5f, 0.2f);
            Gizmos.DrawSphere(eye.transform.position, calculateSightRange());
        }

        private void OnDrawGizmosSelected()
        {
            if (!Application.isEditor)
                return;

            Gizmos.color = new Color(1, 0.65f, 0.5f, 0.2f);
            Gizmos.DrawSphere(transform.position + Vector3.up * 0.75f, calculateSightRange());
        }

        void MobileRobotUpdate()
        {
            velocity = mAgent.velocity.magnitude;

            if (Vector3.Distance(transform.position, mAgent.destination) < 0.5f)
                mAgent.enabled = false;

            mAnimator.SetFloat("velocity", velocity / 2.0f);

            if (velocity > 0.1f && mAgent.enabled)
            {
                mDirection = mAgent.velocity.normalized;
            }
            else
            {

                bool closeEnough = false;

                //You used to be able to bamboozle them by running under their legs lol
                if (EnemyManager.Instance.currentEnemyPhase == ENEMY_PHASE.PHASE_ALERT && Vector3.Distance(transform.position, EnemyManager.Instance.playerActualLocation) < 8.0f)
                    closeEnough = true;

                if (hasVisualOnPlayer || closeEnough)
                {
                    mDirection = Helper.getDirectionTo(transform.position, EnemyManager.Instance.lastPlayerLocation + EnemyManager.Instance.playerMovementDelta * 16.0f);

                    float angle = -Vector3.Angle(eye.forward, Helper.getDirectionTo(eye.position, PlayerManager.Instance.mPlayerInstance.transform.position + Vector3.up * 0.5f));

                    mAnimator.SetFloat("aimAngle", angle / 45.0f);

                    if (mGun)
                    {
                        mGun.updateTargetPos(EnemyManager.Instance.playerActualLocation + (Vector3.up * Random.Range(0.3f, 0.8f)) + EnemyManager.Instance.playerMovementDelta * 15.5f);
                    }
                }
            }

        }

        void StationaryRobotUpdate()
        {

            bool closeEnough = false;

            //You used to be able to bamboozle them by running under their legs lol
            if (EnemyManager.Instance.currentEnemyPhase == ENEMY_PHASE.PHASE_ALERT && Vector3.Distance(transform.position, EnemyManager.Instance.playerActualLocation) < 8.0f)
                closeEnough = true;

            if (hasVisualOnPlayer || closeEnough)
            {
                mDirection = Helper.getDirectionTo(transform.position, EnemyManager.Instance.lastPlayerLocation + EnemyManager.Instance.playerMovementDelta * 16.0f);

                float angle = -Vector3.Angle(eye.forward, Helper.getDirectionTo(eye.position, PlayerManager.Instance.mPlayerInstance.transform.position + Vector3.up * 0.5f));

                mAnimator.SetFloat("aimAngle", angle / 45.0f);

                if (mGun)
                {
                    mGun.updateTargetPos(EnemyManager.Instance.playerActualLocation + (Vector3.up * Random.Range(0.3f, 0.8f)) + EnemyManager.Instance.playerMovementDelta * 15.5f);
                }
            }

        }

        // Update is called once per frame
        void Update()
        {

            lastVisionCheck = visionCheck();

            shootTimer -= Time.deltaTime;


            if (!isStationaryRobot && mAgent != null)
                MobileRobotUpdate();
            else
                StationaryRobotUpdate();

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

            currentMode = Timing.RunCoroutine(Mode_Pursuit().CancelWith(gameObject));
        }

        public void CautionMode()
        {
            killModeCoroutines();

            //currentMode = Timing.RunCoroutine(Mode_Caution());
        }

        public void EvadeMode()
        {
            killModeCoroutines();
            
            currentMode = Timing.RunCoroutine(Mode_Evasion().CancelWith(gameObject));
        }

        public void PatrolMode()
        {
            isAlertMode = false;
            killModeCoroutines();

            if(!isStationaryRobot)
                currentMode = Timing.RunCoroutine(Mode_Patrol().CancelWith(gameObject));
            else
                currentMode = Timing.RunCoroutine(Mode_Vigilance().CancelWith(gameObject));
        }

        private void LateUpdate()
        {
            Vector3 xz_dir = mDirection;

            xz_dir.y = 0;
            xz_dir = xz_dir.normalized;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(xz_dir), Time.deltaTime * 6.0f);

            Debug.DrawLine(eye.position, eye.position + eye.forward * sightRangeNormal);

            pushOutOfOtherRobots();
            snapToGround();

        }

        public IEnumerator<float> Mode_Vigilance()
        {
            //Just keep an eye out for hedgie girl, no need to get up and move.

            while (!isDead)
            {

                if (lastVisionCheck == VisionResult.Close)
                {
                    Exclamation();
                }

                yield return 0f;
            }
        }

        public IEnumerator<float> Mode_Patrol()
        {
            //Just move from waypoint to waypoint, sometimes stopping to look around.

            Debug.Log("Current mode - Patrol");

            int iterator = 0;
            Waypoint mWay = homePatrolRoute[0];

            if (mWay == null)
            {
                homePatrolRoute[0] = new GameObject("WayPoint", typeof(Waypoint)).GetComponent<Waypoint>();
                homePatrolRoute[0].transform.position = transform.position;
                mWay = homePatrolRoute[0];
            }

            float dist = Vector3.Distance(transform.position, mWay.transform.position);

            mAgent.speed = 3.0f;


            while (!isDead)
            {
                subAction = Timing.RunCoroutine(goToWaypoint(homePatrolRoute[iterator]).CancelWith(gameObject));

                while(subAction.IsRunning && subAction.IsValid)
                {

                    if(lastVisionCheck == VisionResult.Far)
                    {
                        CoroutineHandle searchRoutine = Timing.RunCoroutine(investigateLocation(EnemyManager.Instance.lastPlayerLocation, 0.1f).CancelWith(gameObject));

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
            float losingTargetTimeout = 5.0f;

            Debug.Log("Current mode - Pursuit");

            shootTimer = Random.Range(shootTime_Min, shootTime_Max);

            while (!isDead)
            {

                float dist = Vector3.Distance(transform.position, EnemyManager.Instance.playerActualLocation);

                if (dist < 4.0f)
                    losingTarget = false;

                if (lastVisionCheck == VisionResult.None && noVisionFrames > 256)
                {
                    losingTarget = true;
                    losingTargetTimeout = 3.0f;
                }

                losingTargetTimeout -= Time.deltaTime;

                if (lastVisionCheck == VisionResult.Close && losingTargetTimeout <= 0.0f)
                {
                    losingTarget = false;
                }

                if(losingTarget)
                {
                    if (!isStationaryRobot && mAgent != null)
                    {
                        mAgent.enabled = true;
                        mAgent.SetDestination(EnemyManager.getClosestValidDestination(transform.position, EnemyManager.Instance.lastPlayerLocation));
                    }
                }
                else
                {
                    if (!isStationaryRobot && mAgent != null)
                    {
                        mAgent.enabled = false;
                    }
                }




                if (lastVisionCheck != VisionResult.None)
                {
                    EnemyManager.Instance.resetAlertTimer();
                    mAnimator.SetBool("isAiming", true);

                    

                    if (shootTimer <= 0.0f)
                    {
                        Debug.Log("SHOOT!");

                        if (Random.Range(0, 100) < 75.0f)
                        {
                            mGun.fireSingle();
                        }
                        else
                        {
                            mGun.fireBurst(Random.Range(3,5));
                        }


                        shootTimer = Random.Range(shootTime_Min, shootTime_Max);
                    }
                    

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
            if (isStationaryRobot)
                yield break;

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
            if (isStationaryRobot)
                yield break;
           
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

        void pushOutOfOtherRobots()
        {
            //PERSONAL SPACE

            Vector3 myPos = transform.position + Vector3.up;

            foreach(Robot r in FindObjectsOfType<Robot>())
            {
                if (r != this)
                {
                    Vector3 roboPos = r.transform.position + Vector3.up;

                    float dist = Vector3.Distance(myPos, roboPos);

                    if (dist < 3.0f)
                    {
                        transform.position = transform.position - Helper.getDirectionTo(myPos, roboPos) * (dist * Time.deltaTime);
                    }

                }
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