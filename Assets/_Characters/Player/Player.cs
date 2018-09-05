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
	public class Player : MonoBehaviour 
	{
		//TODO change this way of accessing the layer
		[SerializeField] float baseDamage = 10f;
		[SerializeField] Weapon currentWeaponConfig;
		[SerializeField] AnimatorOverrideController animatorOverrideController;
		[Range(0.1f, 1f)] [SerializeField] float criticalHitChance = .1f;
		[SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem critAttackParticles;

        const string ATTACK_TRIGGER = "Attack";
        const string DEFAULT_ATTACK = "DEFAULT ATTACK";

        GameObject weaponObject;
        Enemy enemy;
		Animator animator;
        SpecialAbilities abilities;
		
		CameraRaycaster cameraRaycaster;
		float lastHitTime = 0;
        
		void Start()
		{
            abilities = GetComponent<SpecialAbilities>();

            RegisterForMouseClick ();
			PutWeaponInHand (currentWeaponConfig);
			SetAttackAnimation ();
		}

        private void Update()
        {
            var healthAsPercentage = GetComponent<HealthSystem>().healthAsPercentage;
            if(healthAsPercentage > Mathf.Epsilon)
            {
                ScanForAbilityKeyDown();
            }
        }

        private void ScanForAbilityKeyDown()
        {
            for(int keyIndex = 1; keyIndex < abilities.GetNumberOfAbilities(); keyIndex++)
            {
                if( Input.GetKeyDown(keyIndex.ToString()))
                {
                    abilities.AttemptSpecialAbility(keyIndex);
                }
            }
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
       
		private void AttackTarget()
		{
			if (Time.time - lastHitTime > currentWeaponConfig.GetFireRate())
			{
                SetAttackAnimation();
				animator.SetTrigger (ATTACK_TRIGGER);
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
				abilities.AttemptSpecialAbility (0);
			}
		}

		private void RegisterForMouseClick ()
		{
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			cameraRaycaster.notifyMouseOverEnemyObservers += OnMouseOverEnemy;
			//subscribe to the click observer, that tells us where the player clicked, in the world
		}
	}
}