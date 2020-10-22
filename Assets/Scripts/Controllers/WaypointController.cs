using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointController : MonoBehaviour
{
    public enum WaypointListType
    {
        REVERSE,
        LOOP,
        NONE // enemy animator doesn't support this yet :V
    }

    NavMeshAgent NMAgent;

    [SerializeField] List<Transform> waypoints = new List<Transform>();
    [SerializeField] WaypointListType waypointListType = WaypointListType.REVERSE;

    int currentWaypointIdx = 0;
    bool isReversing = false;

    void Awake()
    {
        NMAgent = GetComponent<NavMeshAgent>();

        if (waypointListType != WaypointListType.NONE)
            NMAgent.destination = waypoints[currentWaypointIdx].position;

        NMAgent.isStopped = true;
    }

    public void ResumePatrol()
    {
        if (waypointListType != WaypointListType.NONE)
            NMAgent.destination = waypoints[currentWaypointIdx].position;
    }

    public void TraverseWaypoints()
    {
        if (waypointListType == WaypointListType.NONE)
            return;

        // check if at target waypoint
        if (!NMAgent.pathPending && NMAgent.remainingDistance <= NMAgent.stoppingDistance)
        {
            // get next waypoint index based on type
            switch (waypointListType)
            {
                case WaypointListType.REVERSE:
                    currentWaypointIdx += isReversing ? -1 : 1;

                    if (currentWaypointIdx == 0 || currentWaypointIdx == waypoints.Count - 1)
                        isReversing = !isReversing;

                    break;

                case WaypointListType.LOOP:
                    ++currentWaypointIdx;

                    if (currentWaypointIdx >= waypoints.Count)
                        currentWaypointIdx = 0;

                    break;
            }

            // go to waypoint
            NMAgent.destination = waypoints[currentWaypointIdx].position;
        }
    }
}