using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Weapons
{
	[CreateAssetMenu(menuName = ("RPG/Weapon"))]
	public class Weapon : ScriptableObject {

		public Transform gripTransform; 

		[SerializeField] GameObject weaponPrefab;
		[SerializeField] AnimationClip attackAnimation;
		[SerializeField] float minSecondsBetweenHits = 0.5f;
		[SerializeField] float maxAttackRange = 2f;
		[SerializeField] float additionalDamage = 10f;

		public GameObject GetWeaponPrefab()
		{
			return weaponPrefab;
		}

		public AnimationClip GetAnimClip()
		{
			RemoveAnimationEvents ();
			return attackAnimation;
		}

		public float GetFireRate()
		{
			return minSecondsBetweenHits;
		}
		 
		public float GetAttackRange()
		{
			return maxAttackRange;
		}

        public float GetAdditionalDamage()
        {
            return additionalDamage;
        }

		// so that asset packs cannot cause crashes
		private void RemoveAnimationEvents()
		{
			attackAnimation.events = new AnimationEvent[0];
		}

	}
}