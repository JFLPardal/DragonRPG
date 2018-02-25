using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Characters
{
	public abstract class SpecialAbilityConfig : ScriptableObject 
	{
		[Header("Special Ability General")] 
		[SerializeField] float energyCost;

		abstract public ISpecialAbility AddComponent (GameObject gameObjectToAttachTo);
	}
}