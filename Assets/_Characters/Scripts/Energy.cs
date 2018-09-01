using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour 
	{
		[SerializeField] Image energyOrb = null;
		[SerializeField] float maxEnergyPoints = 100f;
		[SerializeField] float regenPointsPerSecond = 10f;

		float currentEnergyPoints;

		void Start()
		{
			currentEnergyPoints = maxEnergyPoints;
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

		public bool IsEnergyAvailable(float amount)
		{
			return amount <= currentEnergyPoints;
		}
			
		public void ConsumeEnergy (float amount)
		{
			float newEnergyPoints = currentEnergyPoints - amount;
			currentEnergyPoints = Mathf.Clamp (newEnergyPoints, 0, maxEnergyPoints);
			UpdateEnergyBar ();
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
            energyOrb.fillAmount = EnergyHasPercentage();
		}

		float EnergyHasPercentage()
		{
			return currentEnergyPoints / maxEnergyPoints;
		}
	}
}
