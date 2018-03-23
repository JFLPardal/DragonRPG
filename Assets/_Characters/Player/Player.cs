using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

//TODO consider getting rid of this dependency
using RPG.CameraUI;
using RPG.Core;
using RPG.Weapons;
//***********


namespace RPG.Characters
{
	[RequireComponent(typeof(AudioSource))]
	public class Player : MonoBehaviour, IDamageable 
	{

		//TODO change this way of accessing the layer
		[SerializeField] float maxHealthPoints = 100f; 
		[SerializeField] float baseDamage = 10f;
		[SerializeField] Weapon weaponInUse = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;
		[SerializeField] AudioClip[] damageSounds;
		[SerializeField] AudioClip[] deathSounds;

		//temporarily serialized for dubbing
		[SerializeField] SpecialAbility[] abilities;

        const string DEATH_TRIGGER = "Death";
        const string ATTACK_TRIGGER = "Attack";

		AudioSource audioSource;
		Animator animator;
		float currentHealthPoints;
		CameraRaycaster cameraRaycaster;
		float lastHitTime = 0f;

		public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; }}

		void Start()
		{
			RegisterForMouseClick ();
			SetCurrentMaxHealth ();
			PutWeaponInHand ();
			SetupRuntimeAnimator ();
			abilities[0].AttachComponentTo (gameObject);
			audioSource = GetComponent<AudioSource> ();
		}

		void IDamageable.TakeDamage(float damage)
		{

			bool playerDies = (currentHealthPoints - damage <= 0);
			ReduceHealth (damage);
			if (playerDies) 
			{
				StartCoroutine (KillPlayer ());
			}
		}

		IEnumerator KillPlayer()
		{
            animator.SetTrigger(DEATH_TRIGGER);

            PlayDyingSound ();
			yield return new WaitForSecondsRealtime (audioSource.clip.length);

			SceneManager.LoadScene (0);
		}

		private void ReduceHealth(float damage)
		{
			currentHealthPoints = Mathf.Clamp (currentHealthPoints - damage, 0f, maxHealthPoints);
			audioSource.PlayOneShot (damageSounds[Random.Range(0, damageSounds.Length)]);
			//play animation
		}

		private void AttackTarget(Enemy enemy)
		{
			if (Time.time - lastHitTime > weaponInUse.GetFireRate())
			{
				//TODO make const
				animator.SetTrigger (ATTACK_TRIGGER);	
				enemy.TakeDamage (baseDamage);
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

		void OnMouseOverEnemy(Enemy enemy)
		{
			if(Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
			{
				AttackTarget (enemy);
			}
			else if(Input.GetMouseButtonDown(1))
			{
				AttemptSpecialAbility (0, enemy);
			}
		}

		private void AttemptSpecialAbility(int abilityIndex, Enemy enemy)
		{
			var energyComponent = GetComponent<Energy> ();
			float energyCost = abilities [abilityIndex].GetEnergyCost ();

			if(energyComponent.IsEnergyAvailable(energyCost))
			{
				energyComponent.ConsumeEnergy (energyCost);
				var abilityParams = new AbilityUseParams (enemy, baseDamage);
				abilities [abilityIndex].Use (abilityParams);
			}
		}

		private void RegisterForMouseClick ()
		{
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.notifyMouseOverEnemyObservers += OnMouseOverEnemy;
			//subscribe to the click observer, that tells us where the player clicked, in the world
		}

		private void PlayDyingSound()
		{
			if (audioSource.isPlaying)
				audioSource.Stop ();
			audioSource.clip = deathSounds[Random.Range(0, deathSounds.Length)];
			audioSource.Play();
		}
	}
}