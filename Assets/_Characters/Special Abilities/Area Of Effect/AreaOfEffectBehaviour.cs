using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
	public class AreaOfEffectBehaviour : MonoBehaviour , ISpecialAbility
	{
		AreaOfEffectConfig config;

		public void SetConfig(AreaOfEffectConfig configToSet)
		{
			this.config = configToSet;
		}

		public void Use(AbilityUseParams useParams)
		{
			RaycastHit[] hits = Physics.SphereCastAll (
				                    transform.position,
				                    config.GetAbilityRadius (),
				                    Vector3.up,
				                    config.GetAbilityRadius ()
			                    );

			foreach(RaycastHit hit in hits)
			{
				var damageable = hit.collider.gameObject.GetComponent<IDamageable> ();
				if(damageable != null)
				{
					float damageToDealt = useParams.baseDamage + config.GetDamageToEachTarget ();
					damageable.TakeDamage (damageToDealt);
				}
			}
			print ("AoE activated");
		}
	}
}
