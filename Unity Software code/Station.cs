using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Station : MonoBehaviour
{
    public GameObject stationedRobot;
    public bool isFree;

    // Update is called once per frame
    void Update()
    {
        if (stationedRobot != null)
        {
            // If the robot has more requests in its queue
            if (stationedRobot.GetComponent<Robot>().GetNumOfRequests() > 0)
            {
                stationedRobot = null;
                isFree = true;
                return;
            }

            if (stationedRobot.GetComponent<Robot>().stationed == false)
            {
                // Move the robot to the station
                stationedRobot.GetComponent<Robot>().stationed = true;
                stationedRobot.GetComponent<Robot>().agent.SetDestination(transform.position);
                isFree = false;
            }
        }
    }
}
