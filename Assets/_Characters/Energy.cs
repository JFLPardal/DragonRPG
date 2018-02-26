using UnityEngine;
using UnityEngine.UI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour 
	{
		[SerializeField] RawImage energyBar = null;
		[SerializeField] float maxEnergyPoints = 100f;

		float currentEnergyPoints;

		void Start()
		{
			currentEnergyPoints = maxEnergyPoints;
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

		void UpdateEnergyBar ()
		{
			// TODO remove magic numbers
			float xValue = -(EnergyHasPercentage() / 2f) - 0.5f;
			energyBar.uvRect = new Rect (xValue, 0f, 0.5f, 1f);
		}

		float EnergyHasPercentage()
		{
			return currentEnergyPoints / maxEnergyPoints;
		}
	}
}
