using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RobotController : MonoBehaviour
{

    public Button requestButton;
    public TMP_Dropdown dropdown1;
    public TMP_Dropdown dropdown2;

    public List<GameObject> robots;

    public List<Station> stations;

    public List<GameObject> labelPos;
    public List<TMP_Text> labels;

    // Start is called before the first frame update
    void Start()
    {
        requestButton.onClick.AddListener(OnRequestButtonClick);

        // Initially set the positions for all of the room labels
        for (int i = 0; i < labels.Count; i++)
        {
            labels[i].transform.position = Camera.main.WorldToScreenPoint(labelPos[i].transform.position, Camera.MonoOrStereoscopicEye.Mono);
            labels[i].text = labelPos[i].name;
        }

        
    }

    // Sends a robot to a free station
    public void StationRobot(GameObject robot)
    {
        for (int j = 0; j < stations.Count; j++)
        {
            if (stations[j].stationedRobot == null)
            {
                Debug.Log("Station robot");
                stations[j].stationedRobot = robot;
                return;
            }
        }
    }

    void OnRequestButtonClick()
    {
        // Ensure there is at least 1 robot deployed
        if (robots.Count <= 0)
        {
            Debug.Log("No Robots");
            return;
        }

        Debug.Log("Requested " + dropdown1.options[dropdown1.value].text +
                  " be sent to " + dropdown2.options[dropdown2.value].text);

        string obj = dropdown1.options[dropdown1.value].text;
        GameObject destination = labelPos[dropdown2.value];

        // Recalculated the total queued distance of all the robots
        // before calculated which one has the least amount of work to do
        for (int i = 0; i < robots.Count; i++)
        {
            robots[i].GetComponent<Robot>().UpdateTotalQueuedDist();
        }

        // Calculate which robot has the least amount of work to do
        int bestRobotIndex = 0;

        for (int i = 1; i < robots.Count; i++)
        {
            if (robots[i].GetComponent<Robot>().GetTotalQueuedDist() < robots[bestRobotIndex].GetComponent<Robot>().GetTotalQueuedDist())
            {
                bestRobotIndex = i;
            }
        }

        // Give the robot with the least amount of work the new request
        robots[bestRobotIndex].GetComponent<Robot>().EnqueueRequest(new Request(destination, obj));
    }

}
