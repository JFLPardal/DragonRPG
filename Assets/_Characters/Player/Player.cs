using System.Collections;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

//TODO consider getting rid of this dependency
using RPG.CameraUI;
using RPG.Core;
using System;
//***********


namespace RPG.Characters
{
	[RequireComponent(typeof(AudioSource))]
	public class Player : MonoBehaviour, IDamageable 
	{

		//TODO change this way of accessing the layer
		[SerializeField] float maxHealthPoints = 100f; 
		[SerializeField] float baseDamage = 10f;
		[SerializeField] Weapon currentWeaponConfig = null;
		[SerializeField] AnimatorOverrideController animatorOverrideController = null;
		[SerializeField] AudioClip[] damageSounds;
		[SerializeField] AudioClip[] deathSounds;
		[Range(0.1f, 1f)] [SerializeField] float criticalHitChance = .1f;
		[SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem critAttackParticles = null;

        //temporarily serialized for dubbing
        [SerializeField] AbilityConfig[] abilities;

        const string DEATH_TRIGGER = "Death";
        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        GameObject weaponObject = null;
        Enemy enemy = null;
		AudioSource audioSource = null;
		Animator animator = null;
		float currentHealthPoints = 0;
		CameraRaycaster cameraRaycaster = null;
		float lastHitTime = 0;

		public float healthAsPercentage { get { return currentHealthPoints / maxHealthPoints; }}

		void Start()
		{
            audioSource = GetComponent<AudioSource>();

            RegisterForMouseClick ();
			SetCurrentMaxHealth ();
			PutWeaponInHand (currentWeaponConfig);
			SetAttackAnimation ();
            AttachInitialAbilities();
		}

        private void Update()
        {
            if(healthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
            }
        }

        private void ScanForAbilityKeyDown()
        {
            for(int keyIndex = 1; keyIndex < abilities.Length; keyIndex++)
            {
                if( Input.GetKeyDown(keyIndex.ToString()))
                {
                    AttemptSpecialAbility(keyIndex);
                }
            }
        }

        private void AttachInitialAbilities()
        {
            for(int abilitiesIndex = 0; abilitiesIndex < abilities.Length; abilitiesIndex++)
            {
                abilities[abilitiesIndex].AttachAbilityTo(gameObject);
            }
        }

        public void TakeDamage(float changeAmount)
		{
            currentHealthPoints = Mathf.Clamp(currentHealthPoints - changeAmount, 0f, maxHealthPoints);
            audioSource.PlayOneShot(damageSounds[UnityEngine.Random.Range(0, damageSounds.Length)]);
            if (currentHealthPoints <= 0) 
			{
				StartCoroutine (KillPlayer ());
			}
		}

        public void Heal(float healAmountInPoints)
        {
            currentHealthPoints = Mathf.Clamp(currentHealthPoints + healAmountInPoints, 0f, maxHealthPoints);
        }

        public void PutWeaponInHand(Weapon weaponConfig)
        {
            currentWeaponConfig = weaponConfig;
            var weaponPrefab = weaponConfig.GetWeaponPrefab();
            GameObject dominantHand = RequestDominantHand();
            Destroy(weaponObject); //empty hands
            weaponObject = Instantiate(weaponPrefab, dominantHand.transform);
            weaponObject.transform.localPosition = currentWeaponConfig.gripTransform.localPosition;
            weaponObject.transform.localRotation = currentWeaponConfig.gripTransform.localRotation;
        }

		IEnumerator KillPlayer()
		{
            animator.SetTrigger(DEATH_TRIGGER);

            PlayDyingSound ();
			yield return new WaitForSecondsRealtime (audioSource.clip.length);

			SceneManager.LoadScene (0);
		}
       
		private void AttackTarget()
		{
			if (Time.time - lastHitTime > currentWeaponConfig.GetFireRate())
			{
                SetAttackAnimation();
				animator.SetTrigger (ATTACK_TRIGGER);	
				enemy.TakeDamage (CalculateDamage());
				lastHitTime = Time.time;
			}
		}

        private float CalculateDamage()
        {
            bool isCriticalHit = UnityEngine.Random.Range(0f, 1f) < criticalHitChance;
            float damageBeforeCritical = baseDamage + currentWeaponConfig.GetAdditionalDamage();
            if (isCriticalHit)
            {
                critAttackParticles.Play();
                return damageBeforeCritical * criticalHitMultiplier;
            }
            else
            {
                return damageBeforeCritical;
            }
        }

		private bool IsTargetInRange(GameObject target)
		{
			float distanceToTarget = (target.transform.position - transform.position).magnitude;
			return distanceToTarget <= currentWeaponConfig.GetAttackRange();
		}

		private void SetAttackAnimation()
		{
			animator = GetComponent<Animator> ();
			animator.runtimeAnimatorController = animatorOverrideController;
			animatorOverrideController [DEFAULT_ATTACK] = currentWeaponConfig.GetAnimClip ();
		}

		private void SetCurrentMaxHealth()
		{
			currentHealthPoints = maxHealthPoints;
		}

		private GameObject RequestDominantHand()
		{
			var dominantHands = GetComponentsInChildren<DominantHand> ();
			int numberOfDominantsHands = dominantHands.Length;
			Assert.AreNotEqual (numberOfDominantsHands, 0, "No Dominant Hands Found");
			Assert.IsFalse (numberOfDominantsHands > 1, "Multiple dominant hand scripts on player, remove one");
			return dominantHands [0].gameObject;

		}

		void OnMouseOverEnemy(Enemy enemyToSet)
		{
            enemy = enemyToSet;
			if(Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
			{
				AttackTarget ();
			}
			else if(Input.GetMouseButtonDown(1))
			{
				AttemptSpecialAbility (0);
			}
		}

		private void AttemptSpecialAbility(int abilityIndex)
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
			audioSource.clip = deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)];
			audioSource.Play();
		}
	}
}