using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Core;

namespace RPG.Characters
{
	public struct AbilityUseParams
	{
		public IDamageable target;
		public float baseDamage;

		public AbilityUseParams(IDamageable target, float baseDamage)
		{
			this.target = target;
			this.baseDamage = baseDamage;
		}
	}

	public abstract class AbilityConfig : ScriptableObject 
	{
		[Header("Special Ability General")] 
		[SerializeField] float energyCost;
		[SerializeField] GameObject particlePrefab = null;
        [SerializeField] AudioClip sound = null;

		protected ISpecialAbility behaviour; // only children can set this field

		abstract public void AttachComponentTo (GameObject gameObjectToAttachTo);

		public void Use(AbilityUseParams useParams)
		{
			behaviour.Use (useParams);
		}

		public float GetEnergyCost()
		{
			return energyCost;
		}

        public GameObject GetParticlePrefab()
        {
            return particlePrefab;
        }

        public AudioClip GetAudioClip()
        {
            return sound;
        }
	}

	public interface ISpecialAbility
	{
		void Use(AbilityUseParams useParams);
	}
}