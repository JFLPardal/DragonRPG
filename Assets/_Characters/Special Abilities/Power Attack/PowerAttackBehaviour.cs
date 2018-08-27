using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public class PowerAttackBehaviour : AbilityBehaviour
	{   
		public override void Use(AbilityUseParams useParams)
        {
            PlayAbilitySound();
            PlayParticleEffect();
            DealDamage(useParams);
        }
        

        private void DealDamage(AbilityUseParams useParams)
        {
            float damageToDeal = useParams.baseDamage + (config as PowerAttackConfig).GetExtraDamage();
            useParams.target.TakeDamage(damageToDeal);
        }
    }
}
