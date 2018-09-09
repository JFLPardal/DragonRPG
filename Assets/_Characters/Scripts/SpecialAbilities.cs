using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class SpecialAbilities : MonoBehaviour
    {
        //temporarily serialized for debugging
        [SerializeField] AbilityConfig[] abilities;
        [SerializeField] Image energyBar;
		[SerializeField] float maxEnergyPoints = 100f;
		[SerializeField] float regenPointsPerSecond = 10f;
        [SerializeField] AudioClip outOfEnergy;

        float currentEnergyPoints;
        AudioSource audioSource;
        
        float GetEnergyHasPercentage()
        {
            return currentEnergyPoints / maxEnergyPoints;
        }

        void Start()
		{
            audioSource = GetComponent<AudioSource>();

			currentEnergyPoints = maxEnergyPoints;
            AttachInitialAbilities();
            UpdateEnergyBar();
		}

		void Update()
		{
			if(!IsEnergyFull())
			{
				RegenerateEnergyPoints ();
				UpdateEnergyBar ();
			}
		}

        public int GetNumberOfAbilities()
        {
            return abilities.Length;
        }
        
        private void AttachInitialAbilities()
        {
            for (int abilitiesIndex = 0; abilitiesIndex < abilities.Length; abilitiesIndex++)
            {
                abilities[abilitiesIndex].AttachAbilityTo(gameObject);
            }
        }
        			
		public void ConsumeEnergy (float amount)
		{
			float newEnergyPoints = currentEnergyPoints - amount;
			currentEnergyPoints = Mathf.Clamp (newEnergyPoints, 0, maxEnergyPoints);
			UpdateEnergyBar ();
		}
        
        public void AttemptSpecialAbility(int abilityIndex, GameObject target = null)
        {
            float energyCost = abilities[abilityIndex].GetEnergyCost();

            if (currentEnergyPoints >= energyCost)
            {
                ConsumeEnergy(energyCost);
                abilities[abilityIndex].Use(target);
            }
            else
            {
                audioSource.PlayOneShot(outOfEnergy);
            }
        }

        bool IsEnergyFull()
		{
			return currentEnergyPoints >= maxEnergyPoints;
		}

		void RegenerateEnergyPoints()
		{
			float energyPointsRegenerated = regenPointsPerSecond * Time.deltaTime;
			currentEnergyPoints = Mathf.Clamp(currentEnergyPoints + energyPointsRegenerated, 0, maxEnergyPoints);
		}

		void UpdateEnergyBar ()
		{
            energyBar.fillAmount = GetEnergyHasPercentage();
		}
	}
}
