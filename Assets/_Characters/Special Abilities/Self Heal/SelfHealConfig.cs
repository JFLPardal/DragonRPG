using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
    [CreateAssetMenu(menuName ="RPG/Special Ability/Self Heal")]
    public class SelfHealConfig : AbilityConfig
    {
        [Header("Self Heal Specific")]
        [SerializeField] float hpToRecover;

        protected override AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo)
        {
            return objectToAttachTo.AddComponent<SelfHealBehaviour>();
        }

        public float GetHealingAmount()
        {
            return hpToRecover;
        }
    }

}