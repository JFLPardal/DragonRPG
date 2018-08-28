using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu(menuName = ("RPG/Special Ability/Area Of Effect"))]
	public class AreaOfEffectConfig : AbilityConfig 
	{
		[Header("Area Of Effect Specific")]
		[SerializeField] float radius;
		[SerializeField] float damageToEachTarget;

        protected override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<AreaOfEffectBehaviour>();
        }

        public float GetAbilityRadius()
		{
			return radius;
		}

		public float GetDamageToEachTarget()
		{
			return damageToEachTarget;
		}
	}
}
