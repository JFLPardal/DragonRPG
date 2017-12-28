using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.ThirdPerson;

public class Enemy : MonoBehaviour {

	[SerializeField] float maxHealthPoints = 100f;
	[SerializeField] float attackRadius = 5f;

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
		if (distanceToPlayer < attackRadius) // if player is within range, chase him
			aiCharacterControl.SetTarget (player.transform);
		else //stay in position
			aiCharacterControl.SetTarget(this.transform);
			
	}

	public float healthAsPercentage
	{
		get
		{
			return currentHealthPoints / maxHealthPoints;
		}
	}

	/*void OnDrawGizmos()
	{
		Gizmos.color = Color.black;
		Gizmos.DrawWireSphere (transform.position, attackRadius);
	}*/
}
