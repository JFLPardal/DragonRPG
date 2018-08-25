using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{

    public class SelfHealBehaviour : MonoBehaviour, ISpecialAbility
    {
        SelfHealConfig config;
        Player player;

        private void Start()
        {
            player = GetComponent<Player>();
        }

        public void SetConfig(SelfHealConfig configToSet)
        {
            this.config = configToSet;
        }

        public void Use(AbilityUseParams useParams)
        {
            HealSelf(useParams);
        }

        private void HealSelf(AbilityUseParams useParams)
        {
            if (player != null)
                player.AdjustHealth(-config.GetHealingAmount());
            Debug.Log("Self heal activated");
        }
    }

}
