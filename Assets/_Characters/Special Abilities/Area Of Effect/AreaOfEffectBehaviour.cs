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
            DealRadialDamage(useParams);
            PlayParticleEffect();
        }

        private void PlayParticleEffect()
        {
            var prefab = Instantiate(config.GetParticlePrefab(), transform.position, Quaternion.identity);
            ParticleSystem aoeParticleSystem = prefab.GetComponent<ParticleSystem>();
            aoeParticleSystem.Play();
            Destroy(prefab, aoeParticleSystem.main.duration);
        }

        private void DealRadialDamage(AbilityUseParams useParams)
        {
            RaycastHit[] hits = Physics.SphereCastAll(
                                                transform.position,
                                                config.GetAbilityRadius(),
                                                Vector3.up,
                                                config.GetAbilityRadius()
                                            );

            foreach (RaycastHit hit in hits)
            {
                var damageable = hit.collider.gameObject.GetComponent<IDamageable>();
                bool hitPlayer = hit.collider.gameObject.GetComponent<Player>();
                if (damageable != null && !hitPlayer)
                {
                    float damageToDealt = useParams.baseDamage + config.GetDamageToEachTarget();
                    damageable.AdjustHealth(damageToDealt);
                }
            }
            print("AoE activated");
        }
    }
}
