using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Amy
{

    public enum ENEMY_PHASE
    {
        PHASE_SNEAK,
        PHASE_CAUTION,
        PHASE_ALERT,
        PHASE_EVADE
    }

    public class EnemyManager : Singleton<EnemyManager>
    {

        public Vector3 lastPlayerLocation;
        public Vector3 playerActualLocation;
        public Vector3 playerMovementDelta;

        Vector3 pl_lastPos;

        public ENEMY_PHASE currentEnemyPhase = ENEMY_PHASE.PHASE_SNEAK;

        const float alertTimer_max = 10.0f;
        const float cautionTimer_max = 30.0f;

        public float cautionTimer = 0.0f;
        public float alertTimer = 0.0f;


        const float tauntTimerMin = 4.0f;
        const float tauntTimerMax = 8.0f;

        float voiceTauntTimer = 0.0f;

        // Start is called before the first frame update
        void Awake()
        {
            voiceTauntTimer = Random.Range(tauntTimerMin, tauntTimerMax);
        }

        public void Init()
        {
            Debug.Log("Enemy AI Manager initialized!");
        }

        // Update is called once per frame
        void Update()
        {
            if (!PlayerManager.Instance.mPlayerInstance)
                return;



            playerActualLocation = PlayerManager.Instance.mPlayerInstance.transform.position;

            playerMovementDelta = (playerActualLocation - pl_lastPos);

            pl_lastPos = playerActualLocation;

            //Debug.DrawLine(playerActualLocation + Vector3.up * 0.5f, (playerActualLocation + Vector3.up * 0.5f) + (playerMovementDelta), Color.red, 1.1f);

            if (getClosestRobotToPlayer() == null)
            {
                if(currentEnemyPhase != ENEMY_PHASE.PHASE_SNEAK)
                {
                    PatrolAllBotsInArea();
                }
            }

            if(currentEnemyPhase == ENEMY_PHASE.PHASE_ALERT)
            {
                if(alertTimer > 0.0f)
                {
                    alertTimer -= Time.deltaTime;
                }
                else
                {
                    CautionAllBotsInArea();
                }

                if (alertTimer > 0.7f)
                    lastPlayerLocation = playerActualLocation;
            }
            else if (currentEnemyPhase == ENEMY_PHASE.PHASE_CAUTION)
            {
                if (cautionTimer > 0.0f)
                {
                    cautionTimer -= Time.deltaTime;
                }
                else
                {
                    PatrolAllBotsInArea();
                }

                if (voiceTauntTimer > 0.0f)
                    voiceTauntTimer -= Time.deltaTime;

                if (cautionTimer > tauntTimerMax)
                {
                    if(voiceTauntTimer <= 0.0f)
                    {
                        getClosestRobotVoiceToPlayer().playSearchModeTaunt();
                        voiceTauntTimer = Random.Range(tauntTimerMin, tauntTimerMax);
                    }

                }

            }

        }

        public void setPhase(ENEMY_PHASE nPhase)
        {
            if(nPhase == ENEMY_PHASE.PHASE_SNEAK)
            {
                PatrolAllBotsInArea();
            }
        }


        public void resetAlertTimer()
        {
            alertTimer = alertTimer_max;
        }

        public void resetCautionTimer()
        {
            cautionTimer = cautionTimer_max;
        }

        public static Vector3 getClosestValidDestination(Vector3 start, Vector3 dest)
        {
            NavMeshHit myNavHit;
            if (NavMesh.SamplePosition(dest, out myNavHit, 128, -1))
            {
                return myNavHit.position;
            }

            return start;
        }

        public Robot getClosestRobotToPlayer()
        {
            float dist = 512.0f;

            Robot ret = null;

            foreach(Robot r in FindObjectsOfType<Robot>())
            {
                if(Vector3.Distance(playerActualLocation,r.transform.position) < dist)
                {
                    ret = r;
                }
            }

            return ret;
        }

        public RobotVoices getClosestRobotVoiceToPlayer()
        {
            Robot r = getClosestRobotToPlayer();

            if (r == null)
                return null;

            return r.GetComponent<RobotVoices>();
        }

        public void PatrolAllBotsInArea()
        {
            if (currentEnemyPhase == ENEMY_PHASE.PHASE_SNEAK)
                return;

            if (currentEnemyPhase == ENEMY_PHASE.PHASE_CAUTION && getClosestRobotToPlayer())
                getClosestRobotVoiceToPlayer().playPatrolModeVoice();

            GameManager.Instance.playSystemSound(GameManager.Instance.systemData.sfx_mgs_clear);

            if(FindObjectOfType<SceneInfo>())
            {
               // MusicManager.Instance.changeSongs(FindObjectOfType<SceneInfo>().bgmData);
            }

            currentEnemyPhase = ENEMY_PHASE.PHASE_SNEAK;

            foreach (Robot r in FindObjectsOfType<Robot>())
            {
                r.PatrolMode();
            }

        }

        public void CautionAllBotsInArea()
        {
            if (currentEnemyPhase == ENEMY_PHASE.PHASE_CAUTION)
                return;

            if (currentEnemyPhase == ENEMY_PHASE.PHASE_CAUTION)
                getClosestRobotVoiceToPlayer().playSearchModeVoice();

           // MusicManager.Instance.changeSongs(GameManager.Instance.systemData.bgm_evasion,1.0f);

            currentEnemyPhase = ENEMY_PHASE.PHASE_CAUTION;

            resetCautionTimer();

            foreach (Robot r in FindObjectsOfType<Robot>())
            {
                lastPlayerLocation = getClosestValidDestination(r.transform.position, playerActualLocation);
                r.EvadeMode();
            }

        }



        public void AlertAllBotsInArea()
        {

            if (currentEnemyPhase == ENEMY_PHASE.PHASE_ALERT)
                return;

            if (currentEnemyPhase == ENEMY_PHASE.PHASE_SNEAK)
                getClosestRobotVoiceToPlayer().playAlertModeVoice();

           // MusicManager.Instance.changeSongs(GameManager.Instance.systemData.bgm_alert,0.1f);

            resetAlertTimer();

            currentEnemyPhase = ENEMY_PHASE.PHASE_ALERT;

            Vector3 pos = PlayerManager.Instance.mPlayerInstance.transform.position;

            foreach (Robot r in FindObjectsOfType<Robot>())
            {
                lastPlayerLocation = getClosestValidDestination(r.transform.position, playerActualLocation);
                r.AlertMode();
            }
        }

    }
}