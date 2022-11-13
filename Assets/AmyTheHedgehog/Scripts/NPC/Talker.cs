using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using MEC;

namespace Amy
{

    //[RequireComponent(typeof(NPC))]
    public class Talker : MonoBehaviour
    {
        NPC npc;

        Player player;
        Animator mAnimator;
        public CinemachineVirtualCamera vCam;

        public bool useVirtualCamera = false;

        public List<Message> messages;
        public int msg_index = 0;

        public bool special_CorruptStrings;

        Vector3 originalDirection;
        public Transform messagePosition;

        // Use this for initialization
        void Start()
        {
            if (vCam)
                vCam.m_Priority = -1;


            npc = gameObject.GetComponentInChildren<NPC>();
            mAnimator = gameObject.GetComponent<Animator>();

            if (special_CorruptStrings)
            {
                foreach (Message m in messages)
                {
                    for (int s = 0; s < m.messages.Count - 1; s++)
                    {
                        char[] newSTR = new char[128];

                        for (int i = 0; i < 128; i++)
                        {
                            newSTR[i] = (char)Random.Range(0, 255);
                        }

                        Debug.Log(newSTR);
                        m.messages[s] = newSTR.ToString();

                    }

                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (!player)
                player = FindObjectOfType<Player>();
        }

        public void incrementMessage()
        {
            msg_index++;

            if (msg_index > messages.Count - 1)
                msg_index = 0;
        }

        public void setMessageID(int num)
        {
            msg_index = num;

            if (msg_index > messages.Count - 1)
                msg_index = 0;
        }

        private void OnEnable()
        {

        }

        public void Talk()
        {
            Timing.RunCoroutine(doStartTalking());

        }

        IEnumerator<float> doStartTalking()
        {
            if (special_CorruptStrings)
            {
                foreach (Message m in messages)
                {
                    for (int s = 0; s < m.messages.Count - 1; s++)
                    {
                        char[] newSTR = new char[128];

                        for (int i = 0; i < 128; i++)
                        {
                            newSTR[i] = (char)Random.Range(0, 255);
                        }

                        Debug.Log(newSTR);
                        m.messages[s] = new string(newSTR);


                    }

                }
            }


            MessageBox box = FindObjectOfType<MessageBox>();


            if (npc)
            {
                if (Vector3.Dot(-player.transform.forward, transform.forward) < 0.6f)
                {
                    npc.turnLookAt(player.transform);

                    while (Vector3.Dot(-player.transform.forward, transform.forward) < 0.25f)
                    {
                        yield return 0f;
                    }
                }

                //box.sfxSource.PlayOneShot(GameManager.Instance.systemData.messageBoxSounds.AUDIO_TextBox_Open);

                yield return Timing.WaitForSeconds(0.25f);


                if (player.currentMode != PlayerModes.CUTSCENE && !GameManager.Instance.playerInputDisabled && player.currentMode != PlayerModes.LISTENING)
                {
                    player.changeCurrentMode(PlayerModes.LISTENING);
                }
                

                
            }

            box.ShowMessageBox(messages[msg_index], messagePosition.transform.position, this);
        }

        public void startTalking()
        {
            if (mAnimator)
                mAnimator.Play("Mouth_Talking");
        }

        public void stopTalking()
        {
            if (mAnimator)
                mAnimator.Play("Mouth_NotTalking");

        }

        private void OnDrawGizmosSelected()
        {
            if(messagePosition && !Application.isEditor)
            {
                Gizmos.DrawCube(Camera.main.WorldToScreenPoint(messagePosition.transform.position), Vector3.one * 0.35f);
            }
        }
    }
}
