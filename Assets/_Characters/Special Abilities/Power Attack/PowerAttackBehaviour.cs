using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class PowerAttackBehaviour : AbilityBehaviour
	{   
		public override void Use(GameObject target)
        {
            PlayAbilitySound();
            PlayParticleEffect();
            DealDamage(target);
        }
        
        private void DealDamage(GameObject target)
        {
            float damageToDeal =  (config as PowerAttackConfig).GetExtraDamage();
            target.GetComponent<HealthSystem>().TakeDamage(damageToDeal);
        }
    }
}
