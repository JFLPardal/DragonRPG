using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour, IDamageable {

	[SerializeField] float maxHealthPoints = 100f;
	[SerializeField] float attackRadius = 5f;
	[SerializeField] float chaseRadius = 7f;

	float currentHealthPoints = 100f;
	AICharacterControl aiCharacterControl = null;
	GameObject player = null;

	void Start()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		aiCharacterControl = GetComponent<AICharacterControl> ();
	}

	void Update()
	{
		float distanceToPlayer = Vector3.Distance (transform.position, player.transform.position);

		//attack radius
		if (distanceToPlayer < attackRadius) // if player is within range, chase him
			print(gameObject.name + " is attacking player");
		//TODO spawn projectile

		//chase radius
		if (distanceToPlayer < chaseRadius) // if player is within range, chase him
			aiCharacterControl.SetTarget (player.transform);
		else //stay in position
			aiCharacterControl.SetTarget(this.transform);
			
	}

	void IDamageable.TakeDamage(float damage)
	{
		currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
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
