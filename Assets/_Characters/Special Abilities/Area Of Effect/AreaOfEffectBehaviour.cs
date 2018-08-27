using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
	public class AreaOfEffectBehaviour : AbilityBehaviour
	{
      	public override void Use(AbilityUseParams useParams)
        {
            PlayAbilitySound();
            PlayParticleEffect();
            DealRadialDamage(useParams);
        }
        
        private void DealRadialDamage(AbilityUseParams useParams)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                                                transform.position,
                                                (config as AreaOfEffectConfig).GetAbilityRadius(),
                                                Vector3.up,
                                                (config as AreaOfEffectConfig).GetAbilityRadius()
                                            );

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
                if (damageable != null && !hitPlayer)
                {
                    float damageToDealt = useParams.baseDamage + (config as AreaOfEffectConfig).GetDamageToEachTarget();
                    damageable.TakeDamage(damageToDealt);
                }
            }
        }
    }
}
