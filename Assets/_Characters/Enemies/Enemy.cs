﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO check if dependecy makes sense
using RPG.Core;

namespace RPG.Characters
{
	public class Enemy : MonoBehaviour, IDamageable {

	[SerializeField] float maxHealthPoints = 100f;

	[SerializeField] float chaseRadius = 7f;

	[SerializeField] float attackRadius = 5f;
	[SerializeField] float damagePerShot = 2f;
	[SerializeField] float secondsBetweenShots = 0.5f;
	[SerializeField] float secondsBetweenShotsVariation = 0.2f;
	[SerializeField] GameObject projectileToUse;
	[SerializeField] GameObject projectileSocket;
	[SerializeField] Vector3 aimOffset = new Vector3(0, 1f, 0);

	float currentHealthPoints;
	bool isAttacking = false;
	Player player = null;

	void Start()
	{
		currentHealthPoints = maxHealthPoints;
		player = FindObjectOfType<Player>();
	}

	void Update()
	{
			if(player.healthAsPercentage <= Mathf.Epsilon)
			{
				StopAllCoroutines ();   //stop firing behaviour
				Destroy (this);			//stop enemy behaviour
			}
		float distanceToPlayer = Vector3.Distance (transform.position, player.transform.position);

		//attack radius
		if (distanceToPlayer <= attackRadius && !isAttacking) // if player is within range, chase him
		{ 
			isAttacking = true;
            float randomizeSecondsBetweenShots = secondsBetweenShots + Random.Range(-secondsBetweenShotsVariation, secondsBetweenShotsVariation);
			InvokeRepeating ("FireProjectile", 0f, randomizeSecondsBetweenShots); //TODO switch to coroutine
		}
		//stop firing if player is outside of attack radius
		if (distanceToPlayer > attackRadius)
		{
			isAttacking = false;
			CancelInvoke ();
		}
            //chase radius
            if (distanceToPlayer < chaseRadius) // if player is within range, chase him
            {
                //aiCharacterControl.SetTarget (player.transform);
            }
            else //stay in position
            {
                //aiCharacterControl.SetTarget(this.transform);
            }


        }

	//TODO separate out Character firing logic
	void FireProjectile()
	{
		GameObject newProjectile = Instantiate (projectileToUse, projectileSocket.transform.position, Quaternion.identity);
		Projectile projectileComponent = newProjectile.GetComponent<Projectile> ();
		projectileComponent.SetDamage(damagePerShot);	
		projectileComponent.SetShooter (gameObject);


		Vector3 unitVectorToPlayer = ( (player.transform.position + aimOffset) - projectileSocket.transform.position ).normalized;	//calculate and set velocity for each projectile
		float projectileSpeed = projectileComponent.GetDefaultLaunchSpeed();
		newProjectile.GetComponent<Rigidbody> ().velocity = unitVectorToPlayer * projectileSpeed;
	}

	public void TakeDamage(float damage)
	{
		currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
		if (currentHealthPoints <= 0) { Destroy (gameObject); }
	}

	public float healthAsPercentage	{ get { return currentHealthPoints / maxHealthPoints; }}

	void OnDrawGizmos()
	{
		//chase radius
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere (transform.position, chaseRadius);

		//attack radius
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere (transform.position, attackRadius);
	}
}
}