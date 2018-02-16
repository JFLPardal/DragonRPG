using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

//TODO consider getting rid of this dependency
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;
//***********


namespace RPG.Characters
{
	public class Player : MonoBehaviour, IDamageable 
	{
		[SerializeField] float maxHealthPoints = 100f;
		//TODO change this way of accessing the layer
		[SerializeField] int enemyLayer = 9;
		[SerializeField] float damagePerHit = 10f;
		[SerializeField] float minSecondsBetweenHits = 0.5f;
		[SerializeField] float maxAttackRange = 2f;
		[SerializeField] AnimatorOverrideController animatorOverrideController;

		[SerializeField] Weapon weaponInUse;


		float currentHealthPoints;
		CameraRaycaster cameraRaycaster;
		float lastHitTime = 0f;

		public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; }}

		void Start()
		{
			RegisterForMouseClick ();
			SetCurrentMaxHealth ();
			PutWeaponInHand ();
			OverrideAnimatorController ();
		}



		//TODO refactor to reduce number of lines
		void OnMouseClick (RaycastHit raycastHit, int layerHit)
		{
			if(layerHit == enemyLayer)
			{
				// get the clicked enemy
				var enemy = raycastHit.collider.gameObject;	

				//check if enemy is in range
				if((enemy.transform.position - transform.position).magnitude > maxAttackRange)	
					return;

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

		private void OverrideAnimatorController()
		{
			var animator = GetComponent<Animator> ();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController ["DEFAULT ATTACK"] = weaponInUse.GetAnimClip ();
		}

		private void SetCurrentMaxHealth()
		{
			currentHealthPoints = maxHealthPoints;
		}

		private void PutWeaponInHand()
		{
			var weaponPrefab = weaponInUse.GetWeaponPrefab ();
			GameObject dominantHand = RequestDominantHand ();
			var weapon = Instantiate (weaponPrefab, dominantHand.transform);
			weapon.transform.localPosition = weaponInUse.gripTransform.localPosition;
			weapon.transform.localRotation = weaponInUse.gripTransform.localRotation;

		}

		private GameObject RequestDominantHand()
		{
			var dominantHands = GetComponentsInChildren<DominantHand> ();
			int numberOfDominantsHands = dominantHands.Length;
			Assert.AreNotEqual (numberOfDominantsHands, 0, "No Dominant Hands Found");
			Assert.IsFalse (numberOfDominantsHands > 1, "Multiple dominant hand scripts on player, remove one");
			return dominantHands [0].gameObject;

		}

		private void RegisterForMouseClick ()
		{
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.notifyMouseClickObservers += OnMouseClick;
			//subscribe to the click observer, that tells us where the player clicked, in the world
		}
	}
}