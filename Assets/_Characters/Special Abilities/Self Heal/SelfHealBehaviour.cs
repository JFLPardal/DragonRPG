using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    public class SelfHealBehaviour : AbilityBehaviour
    {
        PlayerControl player = null;

        private void Start()
        {
            player = GetComponent<PlayerControl>();
        }

        public override void Use(GameObject target)
        {
            PlayAbilitySound();
            PlayAbilityAnimation();
            PlayParticleEffect();
            HealSelf();
        }

        private void HealSelf()
        {
            var playerHealth = player.GetComponent<HealthSystem>();
            playerHealth.Heal((config as SelfHealConfig).GetHealingAmount());
        }
    }

}
