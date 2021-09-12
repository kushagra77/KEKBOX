using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class Robot : MonoBehaviour
{
    public Camera cam;
    public TMP_Text info;
    public GameObject textPos;
    public GameObject storage;
    public RobotController controller;


    public bool stationed;
    public bool isMoving;


    public NavMeshAgent agent;

    public List<Item> currentItems;
    private Queue<Request> requests;
    private Request currentRequest;

    private float totalQueuedDistance;
    float prevQueuedDistance;
    float lastSectionDist;
    Vector3 lastPos;

    // Adds a request to the request queue
    public void EnqueueRequest(Request request)
    {
        Debug.Log("Queued Request");

        requests.Enqueue(request);
        isMoving = false;

        Vector3 startPos = transform.position;

        // If there is at least 1 currently active request
        // calculate the distance from the last request in
        // the queue to the new requests position
        if (requests.Count > 0) startPos = lastPos;

        NavMeshPath path = new NavMeshPath();

        NavMesh.CalculatePath(startPos, request.GetDestination().transform.position, NavMesh.AllAreas, path);
        float dist = CalculatePathDistance(path);

        // Track the new distance this request creates
        prevQueuedDistance += dist;

        // Record the position of the last request in the queue
        lastPos = request.GetDestination().transform.position;
    }

    // Calculates the distance of a NavMeshPath
    private float CalculatePathDistance(NavMeshPath path)
    {
        // Check that the path forms a line
        if (path.corners.Length < 2) return 0;

        // Iterate through each node in the path and sum the distances
        float totalPathDist = 0;
        for(int i = 1; i < path.corners.Length; i++)
        {
            totalPathDist += Vector3.Distance(path.corners[i - 1], path.corners[i]);

        }

        return totalPathDist;
    }

    public int GetNumOfRequests()
    {
        return requests.Count;
    }


    // Recalculates the sum of the distances from each request
    // Also considers how far a robot has moved 
    public void UpdateTotalQueuedDist()
    {
        // Assert that the robot still needs or is moving
        if (currentRequest == null && !isMoving) return;

        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(transform.position, currentRequest.GetDestination().transform.position, NavMesh.AllAreas, path);
        float newDist = CalculatePathDistance(path);

        // Calculate the new total queued distance
        totalQueuedDistance = (prevQueuedDistance - (lastSectionDist - newDist));
    
    }

    public float GetTotalQueuedDist()
    {
        return totalQueuedDistance;
    }


    // Start is called before the first frame update
    void Start()
    {
        lastPos = Vector3.zero;
        lastSectionDist = 0;

        currentRequest = null;
        stationed = true;

        requests = new Queue<Request>();
        currentItems = new List<Item>();
        
    }

    // Update is called once per frame
    void Update()
    {
        // Reposition the robot's information text to its new location
        info.transform.position = cam.WorldToScreenPoint(textPos.transform.position, Camera.MonoOrStereoscopicEye.Mono);

        // If there are requests in the queue and there are no current active reqests
        if (requests.Count > 0 && currentRequest == null && isMoving == false)
        {
            currentRequest = requests.Peek();

            stationed = false;
            isMoving = true;

            // Check if the robot is carrying the correct item
            bool alreadyHasItem = false;

            // Linear search through all the items on the robot
            for (int i = 0; i < currentItems.Count; i++)
            {
                if (currentItems[i].name == currentRequest.GetItem())
                {
                    alreadyHasItem = true;
                    break;
                }
            }

            // If the item can't be found
            if (alreadyHasItem == false)
            {
                // Add a new request to the front of the queue which directs the
                // robot back to storage to pickup the correct item

                Queue<Request> newRequests = new Queue<Request>();

                newRequests.Enqueue(new Request(storage, currentRequest.GetItem()));

                for (int i = 0; i < requests.Count; i++)
                {
                    newRequests.Enqueue(requests.Dequeue());
                }

                requests = newRequests;

            }

            // Dequeue the first request from the queue
            currentRequest = requests.Dequeue();

            // Move the robot towards the current requests position
            agent.SetDestination(currentRequest.GetDestination().transform.position);

            // Calculate the distance of this request
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, currentRequest.GetDestination().transform.position, NavMesh.AllAreas, path);
            lastSectionDist = CalculatePathDistance(path);
          
        }

        if (currentRequest != null)
        {
            // Display info above the robot when it's completing a request
            if (currentItems.Count > 0 && currentItems.Count > 0)
            {
                info.text = "\nItem: " + currentRequest.GetItem() + "\nDest: " + currentRequest.GetDestination().name;
            }
            else
            {
                // The robot either doesn't have the correct item or it has no requests
                info.text = "\nItem: Nothing" + "\nDest: " + currentRequest.GetDestination().name;
            }

            // If the robot is 1.5 units away from the current requests destination
            if (Vector3.Distance(transform.position, currentRequest.GetDestination().transform.position) < 1.5)
            {

                if (currentRequest.GetDestination() == storage)
                {
                    // If the required item is in storage
                    if (storage.GetComponent<Inventory>().RequestItem(currentRequest.GetItem()))
                    {
                        // Load the item onto the robot
                        currentItems.Add(new Item(currentRequest.GetItem(), 1));
                        Debug.Log("Collected item: Quanity - 1, Name - " + currentRequest.GetItem());
                    }
                }
                else
                {
                    // Remove all items on the robot
                    for (int i = 0; i < currentItems.Count; i++)
                    {
                        currentItems.RemoveAt(i);
                    }
                }
       
                UpdateTotalQueuedDist();
                prevQueuedDistance = totalQueuedDistance;

                isMoving = false;
                currentRequest = null;

                info.text = "Idle";
            } 
            
        } else
        {
            // The requests have all been
            // completed so reset distance data
            prevQueuedDistance = 0;
            lastSectionDist = 0;
            totalQueuedDistance = 0;

        }

        // If the robot has finished all of it's requests and isn't moving back to a station
        if (GetNumOfRequests() == 0 && !isMoving && stationed == false)
        {
            controller.StationRobot(this.gameObject);
        }

              
    }
}
