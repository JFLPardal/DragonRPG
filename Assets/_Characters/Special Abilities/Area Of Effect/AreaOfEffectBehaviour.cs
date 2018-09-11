using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
	public class AreaOfEffectBehaviour : AbilityBehaviour
	{
      	public override void Use(GameObject target)
        {
            PlayAbilitySound();
            PlayAbilityAnimation();
            PlayParticleEffect();
            DealRadialDamage();
        }
        
        private void DealRadialDamage()
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                                                transform.position,
                                                (config as AreaOfEffectConfig).GetAbilityRadius(),
                                                Vector3.up,
                                                (config as AreaOfEffectConfig).GetAbilityRadius()
                                            );

            foreach (RaycastHit hit in hits)
            {
                var enemy = hit.collider.gameObject.GetComponent<HealthSystem>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<PlayerControl>();
                if (enemy != null && !hitPlayer)
                {
                    float damageToDealt = (config as AreaOfEffectConfig).GetDamageToEachTarget();
                    enemy.TakeDamage(damageToDealt);
                }
            }
        }
    }
}
