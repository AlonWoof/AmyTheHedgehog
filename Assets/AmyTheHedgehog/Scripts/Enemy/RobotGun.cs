using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MEC;

public class RobotGun : MonoBehaviour
{

    public List<Transform> muzzlePoints;
    public GameObject projectileToFire;

    int burstFireAmount = 1;
    float burstFireInterval = 0.1f;

    public Vector3 target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void fire()
    {
        //today is friday in california
        //SHOOT

        foreach (Transform t in muzzlePoints)
        {
            GameObject bullet = GameObject.Instantiate(projectileToFire);
            bullet.transform.position = t.position;
            bullet.transform.rotation = Quaternion.LookRotation(Helper.getDirectionTo(t.position,target));
        }

    }

    public void updateTargetPos(Vector3 pos)
    {
        target = pos;

    }

    public void fireSingle()
    {
        fire();
    }

    public void fireBurst()
    {
        fireBurst(burstFireAmount);
    }

    public void fireBurst(int amount)
    {
        Timing.RunCoroutine(doBurstFire(amount));
    }

    IEnumerator<float> doBurstFire(int amount)
    {
        int shots = amount;

        while(shots > 0)
        {
            fire();
            shots--;
            yield return Timing.WaitForSeconds(burstFireInterval);
        }
    }
}
