using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{

    public class SelfHealBehaviour : AbilityBehaviour
    {
        Player player = null;

        private void Start()
        {
            player = GetComponent<Player>();
        }

        public override void Use(AbilityUseParams useParams)
        {
            PlayAbilitySound();
            PlayParticleEffect();
            HealSelf(useParams);
        }

        private void HealSelf(AbilityUseParams useParams)
        {
            var playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.Heal((config as SelfHealConfig).GetHealingAmount());
        }
    }

}
