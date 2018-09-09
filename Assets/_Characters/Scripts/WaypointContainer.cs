using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{ 
    public class WaypointContainer : MonoBehaviour
    {
        float waypointGizmoRadius = .2f;

        public int GetNumberOfWaypoints()
        {
            return transform.childCount;
        }

        void OnDrawGizmos()
        {
            Vector3 firstWaypointPosition = transform.GetChild(0).position;
            Vector3 previousWaypointPosition = firstWaypointPosition;

            foreach(Transform waypoint in transform)
            {
                Gizmos.DrawSphere(waypoint.position, waypointGizmoRadius);
                Gizmos.DrawLine(previousWaypointPosition, waypoint.position);
                previousWaypointPosition = waypoint.position;
            }
            Gizmos.color = Color.red;
            Gizmos.DrawLine(previousWaypointPosition, firstWaypointPosition);
        }
    }
}
