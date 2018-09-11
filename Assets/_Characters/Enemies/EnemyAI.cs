﻿using System;
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

        PlayerMovement player;
        Character character;
        int nextWaypointIndex;
        float currentWeaponRange;
        float distanceToPlayer;

        enum State { idle, attacking, patrolling, chasing }
        State state = State.idle;

        void Start()
	    {
		    player = FindObjectOfType<PlayerMovement>();
            character = GetComponent<Character>();
        }

	    void Update()
	    {
		    distanceToPlayer = Vector3.Distance (transform.position, player.transform.position);
            WeaponSystem weaponSystem = GetComponent<WeaponSystem>();
            currentWeaponRange = weaponSystem.GetCurrentWeapon().GetAttackRange();

            if(distanceToPlayer > chaseRadius && state != State.patrolling)
            {
                StopAllCoroutines();
                StartCoroutine(Patrol());
            }
            if(distanceToPlayer <= chaseRadius && state != State.chasing)
            {
                StopAllCoroutines();
                StartCoroutine(ChasePlayer());
            }
            if(distanceToPlayer <= currentWeaponRange && state != State.attacking)
            {
                StopAllCoroutines();
                state = State.attacking;
                // attack
            }
        }

        IEnumerator Patrol()
        {
            state = State.patrolling;
            while (true)
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