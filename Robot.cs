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

    public List<Item> currentItems;

    public bool stationed;

    public NavMeshAgent agent;
    private Queue<Request> requests;

    private Request currentRequest;

    private float totalQueuedDistance;
    float prevQueuedDistance;
    public bool isMoving;

    Vector3 lastPos;
    float lastSectionDist;

    public void EnqueueRequest(Request request)
    {
        Debug.Log("Queued Request");
        requests.Enqueue(request);
        isMoving = false;
        Vector3 startPos = transform.position;
        
        if (requests.Count > 0) startPos = lastPos;

        NavMeshPath path = new NavMeshPath();

        NavMesh.CalculatePath(startPos, request.GetDestination().transform.position, NavMesh.AllAreas, path);
        float dist = CalculatePathDistance(path);
        //  Debug.Log("TotalQueuedDistance: Before: " + totalQueuedDistance);

        prevQueuedDistance += dist;
        // Debug.Log("TotalQueuedDistance: After: " + totalQueuedDistance);


        lastPos = request.GetDestination().transform.position;
    }

    private float CalculatePathDistance(NavMeshPath path)
    {
        if (path.corners.Length < 2) return 0;

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



    public void UpdateTotalQueuedDist()
    {
        if (currentRequest == null && !isMoving) return;
        NavMeshPath path = new NavMeshPath();

        NavMesh.CalculatePath(transform.position, currentRequest.GetDestination().transform.position, NavMesh.AllAreas, path);
        float newDist = CalculatePathDistance(path);
        totalQueuedDistance = (prevQueuedDistance - (lastSectionDist - newDist));
     //   Debug.Log("TotalQueuedDistance: " + totalQueuedDistance);
     //   Debug.Log("Difference: " + (lastSectionDist - newDist));

       // Debug.Log("lastSectionDist: Before" + lastSectionDist);
        // lastSectionDist -= newDist;
      //  Debug.Log("lastSectionDist: After" + (lastSectionDist - newDist));


    }

    public float GetTotalQueuedDist()
    {
        return totalQueuedDistance;
    }

    

    // Start is called before the first frame update
    void Start()
    {
        //info.transform.position = cam.WorldToScreenPoint(textPos.transform.position, Camera.MonoOrStereoscopicEye.Mono);
        lastPos = Vector3.zero;
        lastSectionDist = 0;
        currentRequest = null;
        requests = new Queue<Request>();
        currentItems = new List<Item>();
        stationed = true;
    }

    // Update is called once per frame
    void Update()
    {

        info.transform.position = cam.WorldToScreenPoint(textPos.transform.position, Camera.MonoOrStereoscopicEye.Mono);

        if (requests.Count > 0 && currentRequest == null && isMoving == false)
        {
            currentRequest = requests.Peek();
            stationed = false;
            isMoving = true;

            // Check if carrying item
            bool alreadyHasItem = false;

            for (int i = 0; i < currentItems.Count; i++)
            {
                if (currentItems[i].name == currentRequest.GetItem())
                {
                    alreadyHasItem = true;
                    break;
                }
            }

            if (alreadyHasItem == false)
            {
                // Add a new request to go back to storage and grab item

                Queue<Request> newRequests = new Queue<Request>();

                newRequests.Enqueue(new Request(storage, currentRequest.GetItem()));

                for (int i = 0; i < requests.Count; i++)
                {
                    newRequests.Enqueue(requests.Dequeue());
                }

                requests = newRequests;

            }
            currentRequest = requests.Dequeue();

            agent.SetDestination(currentRequest.GetDestination().transform.position);

            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, currentRequest.GetDestination().transform.position, NavMesh.AllAreas, path);
            lastSectionDist = CalculatePathDistance(path);
            // prevQueuedDistance = lastSectionDist;

            // Debug.Log("lastSectionDist: " + lastSectionDist);
        }

        if (currentRequest != null)
        {
            //info.text = "Dest: " + currentRequest.GetDestination().name + "\nItem: " + currentRequest.GetItem() + "\nDist: " + totalQueuedDistance;

            if (currentItems.Count > 0 && currentItems.Count > 0)
            {
                info.text = "\nItem: " + currentRequest.GetItem() + "\nDest: " + currentRequest.GetDestination().name;

            }
            else
            {
                info.text = "\nItem: Nothing" + "\nDest: " + currentRequest.GetDestination().name;

            }

            if (Vector3.Distance(transform.position, currentRequest.GetDestination().transform.position) < 1.5)
            {

                if (currentRequest.GetDestination() == storage)
                {
                    if (storage.GetComponent<Inventory>().RequestItem(currentRequest.GetItem()))
                    {
                        currentItems.Add(new Item(currentRequest.GetItem(), 1));
                        Debug.Log("Collected item: Quanity - 1, Name - " + currentRequest.GetItem());
                    }
                }
                else
                {
                    for (int i = 0; i < currentItems.Count; i++)
                    {
                        currentItems.RemoveAt(i);

                       /* currentItems[i].Decrease();
                        if (currentItems[i].GetNum() == 0)
                        {
                           currentItems.RemoveAt(i);
                          
                        }*/
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
            
            prevQueuedDistance = 0;
            lastSectionDist = 0;
            totalQueuedDistance = 0;

        }

        if (GetNumOfRequests() == 0 && !isMoving && stationed == false)
        {
            controller.StationRobot(this.gameObject);
        }

                /*
                if (Input.GetMouseButtonDown(0))
                {
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hit;

                    if (Physics.Raycast(ray, out hit))
                    {

                    }

                }*/
            }
}
