using UnityEngine;
using UnityEngine.Assertions;

using RPG.CameraUI;
using System;

namespace RPG.Characters
{
	public class PlayerMovement : MonoBehaviour 
	{
		//TODO change this way of accessing the layer
		[Range(0.1f, 1f)] [SerializeField] float criticalHitChance = .1f;
		[SerializeField] float criticalHitMultiplier = 1.25f;
        [SerializeField] ParticleSystem critAttackParticles;

        EnemyAI enemy;
        SpecialAbilities abilities;
        Character character;
        WeaponSystem weaponSystem;
		
		CameraRaycaster cameraRaycaster;
        
		void Start()
		{
            abilities = GetComponent<SpecialAbilities>();
            character = GetComponent<Character>();
            weaponSystem = GetComponent <WeaponSystem>();

            RegisterForMouseEvent ();
		}
        
        private void RegisterForMouseEvent()
        {
            cameraRaycaster = FindObjectOfType<CameraRaycaster>();
            cameraRaycaster.notifyMouseOverEnemyObservers += OnMouseOverEnemy;
            cameraRaycaster.notifyMouseOverPotentiallyWalkableObservers += OnMouseOverPotentiallyWalkable;
        }
        
        void OnMouseOverEnemy(EnemyAI enemyToSet)
        {
            enemy = enemyToSet;
            if (Input.GetMouseButton(0) && IsTargetInRange(enemy.gameObject))
            {
                weaponSystem.AttackTarget(enemy.gameObject);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                abilities.AttemptSpecialAbility(0);
            }
        }

        private void OnMouseOverPotentiallyWalkable(Vector3 destination)
        {
            if(Input.GetMouseButton(0))
            {
                character.SetDestination(destination);
            }
        }

        private void Update()
        {
            ScanForAbilityKeyDown();
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

        private bool IsTargetInRange(GameObject target)
        {
            float distanceToTarget = (target.transform.position - transform.position).magnitude;
            return distanceToTarget <= weaponSystem.GetCurrentWeapon().GetAttackRange();
        }
    }
}