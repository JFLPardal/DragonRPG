using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	[CreateAssetMenu(menuName = ("RPG/Special Ability/Area Of Effect"))]
	public class AreaOfEffectConfig : SpecialAbility 
	{
		[Header("Area Of Effect Specific")]
		[SerializeField] float radius;
		[SerializeField] float damageToEachTarget;

		public override void AttachComponentTo (GameObject gameObjectToAttachTo)
		{
			var behaviourComponent = gameObjectToAttachTo.AddComponent<AreaOfEffectBehaviour> ();
			behaviourComponent.SetConfig (this);
			behaviour = behaviourComponent;
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
