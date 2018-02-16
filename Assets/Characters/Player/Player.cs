using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable {


	[SerializeField] float maxHealthPoints = 100f;
	//TODO change this way of accessing the layer
	[SerializeField] int enemyLayer = 9;
	[SerializeField] float damagePerHit = 10f;
	[SerializeField] float minSecondsBetweenHits = 0.5f;
	[SerializeField] float maxAttackRange = 2f;

	[SerializeField] Weapon weaponInUse;
	[SerializeField] GameObject weaponSocket;


	float currentHealthPoints;
	GameObject currentTarget;
	CameraRaycaster cameraRaycaster;
	float lastHitTime = 0f;

	public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; }}

	void Start()
	{
		RegisterForMouseClick ();
		currentHealthPoints = maxHealthPoints;
		PutWeaponInHand ();
	}

	void OnMouseClick (RaycastHit raycastHit, int layerHit)
	{
		if(layerHit == enemyLayer)
		{
			// get the clicked enemy
			var enemy = raycastHit.collider.gameObject;	

			//check if enemy is in range
			if((enemy.transform.position - transform.position).magnitude > maxAttackRange)	
				return;

			currentTarget = enemy;
			var enemyComponent = enemy.GetComponent<Enemy> ();
			if (Time.time - lastHitTime > minSecondsBetweenHits)
			{
				//damage the clicked enemy
				enemyComponent.TakeDamage (damagePerHit);	
				lastHitTime = Time.time;
			}
		}
	}

	void IDamageable.TakeDamage(float damage)
	{
		currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
	}

	void PutWeaponInHand()
	{
		var weaponPrefab = weaponInUse.GetWeaponPrefab ();
		var weapon = Instantiate (weaponPrefab, weaponSocket.transform);
		weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
		weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;

	}

	void RegisterForMouseClick ()
	{
		cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
		cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
		//subscribe to the click observer, that tells us where the player clicked, in the world
	}
}
