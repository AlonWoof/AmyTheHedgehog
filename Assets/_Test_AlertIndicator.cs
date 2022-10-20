using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Amy;

public class _Test_AlertIndicator : MonoBehaviour
{
    public Image alertIndicatorImage;
    public Text alertPhaseLabel;
    public Text alertPhaseTimer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(EnemyManager.Instance.currentEnemyPhase == ENEMY_PHASE.PHASE_SNEAK)
        {
            alertIndicatorImage.color = Color.green;
            alertPhaseLabel.text = "CLEAR";
            alertPhaseTimer.text = "";
        }
        else if (EnemyManager.Instance.currentEnemyPhase == ENEMY_PHASE.PHASE_CAUTION)
        {
            alertIndicatorImage.color = Color.yellow;
            alertPhaseLabel.text = "CAUTION";
            alertPhaseTimer.text = EnemyManager.Instance.cautionTimer.ToString("N1");
        }
        else if (EnemyManager.Instance.currentEnemyPhase == ENEMY_PHASE.PHASE_ALERT)
        {
            alertIndicatorImage.color = Color.red;
            alertPhaseLabel.text = "ALERT";
            alertPhaseTimer.text = EnemyManager.Instance.alertTimer.ToString("N1");
        }

    }
}
