using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu(menuName = ("RPG/Weapon"))]
	public class WeaponConfig : ScriptableObject {

		public Transform gripTransform; 

		[SerializeField] GameObject weaponPrefab;
		[SerializeField] AnimationClip attackAnimation;
		[SerializeField] float timeBetweenAnimationCycles = 0.5f;
		[SerializeField] float maxAttackRange = 2f;
		[SerializeField] float additionalDamage = 10f;
		[SerializeField] float damageDelay = .5f;

		public GameObject GetWeaponPrefab()
		{
			return weaponPrefab;
		}

		public AnimationClip GetAnimClip()
		{
			RemoveAnimationEvents ();
			return attackAnimation;
		}

		public float GetTimeBetweenAnimationCycles()
		{
			return timeBetweenAnimationCycles;
		}
		 
		public float GetAttackRange()
		{
			return maxAttackRange;
		}

        public float GetAdditionalDamage()
        {
            return additionalDamage;
        }

        public float GetDamageDelay()
        {
            return damageDelay;
        }

		// so that asset packs cannot cause crashes
		private void RemoveAnimationEvents()
		{
			attackAnimation.events = new AnimationEvent[0];
		}

	}
}