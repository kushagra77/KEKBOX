using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public GameObject stationedRobot;
    public bool isFree;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (stationedRobot != null)
        {
            if (stationedRobot.GetComponent<Robot>().GetNumOfRequests() > 0)
            {
                stationedRobot = null;
                isFree = true;
                return;
            }

            if (stationedRobot.GetComponent<Robot>().stationed == false)
            {
                stationedRobot.GetComponent<Robot>().stationed = true;
                stationedRobot.GetComponent<Robot>().agent.SetDestination(transform.position);
                isFree = false;
            }
        }
    }
}
