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

		//TODO change this way of accessing the layer
		[SerializeField] int enemyLayer = 9;
		[SerializeField] float maxHealthPoints = 100f; 
		[SerializeField] float damagePerHit = 10f;
		[SerializeField] Weapon weaponInUse = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;

		Animator animator;
		float currentHealthPoints;
		CameraRaycaster cameraRaycaster;
		float lastHitTime = 0f;

		public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; }}

		void IDamageable.TakeDamage(float damage)
		{
			currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
		}

		void Start()
		{
			RegisterForMouseClick ();
			SetCurrentMaxHealth ();
			PutWeaponInHand ();
			SetupRuntimeAnimator ();
		}

		void OnMouseClick (RaycastHit raycastHit, int layerHit)
		{
			if(layerHit == enemyLayer)
			{
				var enemy = raycastHit.collider.gameObject;	
				if(IsTargetInRange(enemy))
				{
					AttackTarget (enemy);
				}
			}
		}

		private void AttackTarget(GameObject target)
		{

			var enemyComponent = target.GetComponent<Enemy> ();
			if (Time.time - lastHitTime > weaponInUse.GetFireRate())
			{
				//TODO make const
				animator.SetTrigger ("Attack");	
				enemyComponent.TakeDamage (damagePerHit);
				lastHitTime = Time.time;
			}
			
		}

		private bool IsTargetInRange(GameObject target)
		{
			float distanceToTarget = (target.transform.position - transform.position).magnitude;
			return distanceToTarget <= weaponInUse.GetAttackRange();
		}

		private void SetupRuntimeAnimator()
		{
			animator = GetComponent<Animator> ();
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