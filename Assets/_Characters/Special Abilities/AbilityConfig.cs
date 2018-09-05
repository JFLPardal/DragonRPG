using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
	public abstract class AbilityConfig : ScriptableObject 
	{
		[Header("Special Ability General")] 
		[SerializeField] float energyCost;
		[SerializeField] GameObject particlePrefab = null;
        [SerializeField] AudioClip[] abilitySounds = null;

		protected AbilityBehaviour behaviour; // only children can set this field

        protected abstract AbilityBehaviour GetBehaviourComponent(GameObject objectToAttachTo);

        public void AttachAbilityTo (GameObject objectToAttachTo)
        {
            AbilityBehaviour behaviourComponent = GetBehaviourComponent(objectToAttachTo);
            behaviourComponent.SetConfig(this);
            behaviour = behaviourComponent;
        }

		public void Use(GameObject target)
		{
			behaviour.Use (target);
		}

		public float GetEnergyCost()
		{
			return energyCost;
		}

        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public AudioClip GetRandomAbilityClip()
        {
            return abilitySounds[UnityEngine.Random.Range(0, abilitySounds.Length)];
        }
	}
}