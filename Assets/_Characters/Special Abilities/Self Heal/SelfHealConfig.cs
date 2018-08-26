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

        
        public override void AttachComponentTo(GameObject gameObjectToAttachTo)
        {
            var selfHealBehaviour = gameObjectToAttachTo.AddComponent<SelfHealBehaviour>();
            selfHealBehaviour.SetConfig(this);
            behaviour = selfHealBehaviour;
        }

        public float GetHealingAmount()
        {
            return hpToRecover;
        }
    }

}