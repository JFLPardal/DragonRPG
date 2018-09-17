using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [RequireComponent(typeof(WeaponSystem))]
    [RequireComponent(typeof(Character))]
    [RequireComponent(typeof(HealthSystem))]
	public class EnemyAI : MonoBehaviour
    {
	    [SerializeField] float chaseRadius = 7f;
	    [SerializeField] WaypointContainer patrolRoute;
        [SerializeField] float waypointStoppingDistance = 1.5f;
        [SerializeField] float timeToWaitInWaypointInSeconds = 1f;

        PlayerControl player;
        Character character;
        int nextWaypointIndex;
        float currentWeaponRange;
        float distanceToPlayer;

        enum State { idle, attacking, patrolling, chasing }
        State state = State.idle;

        void Start()
	    {
		    player = FindObjectOfType<PlayerControl>();
            character = GetComponent<Character>();
        }

	    void Update()
	    {
		    distanceToPlayer = Vector3.Distance (transform.position, player.transform.position);
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetAttackRange();

            bool playerIsInWeaponRadius = distanceToPlayer <= currentWeaponRange;
            bool playerIsInChaseRadius = distanceToPlayer > currentWeaponRange
                                         && distanceToPlayer <= chaseRadius;
            bool playerIsOutsideChaseRadius = distanceToPlayer > chaseRadius;
        
            if(playerIsOutsideChaseRadius && state != State.patrolling && patrolRoute != null)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(Patrol());
            }
            if(playerIsInChaseRadius && state != State.chasing)
            {
                StopAllCoroutines();
                weaponSystem.StopAttacking();
                StartCoroutine(ChasePlayer());
            }
            if(playerIsInWeaponRadius && state != State.attacking)
            {
                StopAllCoroutines();
                state = State.attacking;
                weaponSystem.AttackTarget(player.gameObject);
            }
        }

        IEnumerator Patrol()
        {
            state = State.patrolling;
            while (patrolRoute != null)
            {
                Vector3 nextWaypointPosition = patrolRoute.transform.GetChild(nextWaypointIndex).position;
                character.SetDestination(nextWaypointPosition);
                CycleWaypointWhenClose(nextWaypointPosition);
                yield return new WaitForSeconds(timeToWaitInWaypointInSeconds);                 
            }
        }

        private void CycleWaypointWhenClose(Vector3 nextWaypointPosition)
        {
            if(Vector3.Distance(transform.position, nextWaypointPosition) <= waypointStoppingDistance)
            {
                nextWaypointIndex = (nextWaypointIndex + 1) % patrolRoute.GetNumberOfWaypoints();
            }
        }

        IEnumerator ChasePlayer()
        {
            state = State.chasing;
            while(distanceToPlayer > currentWeaponRange)
            {
                character.SetDestination(player.transform.position);
                yield return new WaitForEndOfFrame();
            }
        }
        
	    void OnDrawGizmos()
	    {
		    //chase radius
		    Gizmos.color = Color.blue;
		    Gizmos.DrawWireSphere (transform.position, chaseRadius);

		    //attack radius
		    Gizmos.color = Color.red;
		    Gizmos.DrawWireSphere (transform.position, currentWeaponRange);
	    }
    }
}