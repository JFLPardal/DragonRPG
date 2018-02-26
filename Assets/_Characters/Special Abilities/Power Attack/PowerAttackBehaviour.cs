using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class PowerAttackBehaviour : MonoBehaviour, ISpecialAbility
	{
		PowerAttackConfig config;

		public void SetConfig(PowerAttackConfig configToSet)
		{
			this.config = configToSet;
		}

		void Start()
		{
		}

		public void Use(AbilityUseParams useParams)
		{
			print ("power attack used by: " + gameObject);
			float damageToDeal = useParams.baseDamage + config.GetExtraDamage ();
			useParams.target.TakeDamage (damageToDeal);
		}
	}
}
