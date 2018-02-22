using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.CameraUI;

namespace RPG.Characters
{
	public class Energy : MonoBehaviour 
	{
		[SerializeField] RawImage energyBar;
		[SerializeField] float maxEnergyPoints = 100f;
		[SerializeField] float pointsPerHit = 10f;

		CameraRaycaster cameraRaycaster = null;
		float currentEnergyPoints;

		void Start()
		{
			currentEnergyPoints = maxEnergyPoints;
			SubscribeRightClick ();
		}

		void SubscribeRightClick ()
		{
			cameraRaycaster = FindObjectOfType<CameraRaycaster> ();
			if (cameraRaycaster != null)
				cameraRaycaster.notifyRightClickObservers += ProcessRightClick;
		}


		private void ProcessRightClick (RaycastHit raycastHit, int layerHit)
		{
			float newEnergyPoints = currentEnergyPoints - pointsPerHit;
			currentEnergyPoints = Mathf.Clamp(newEnergyPoints, 0, maxEnergyPoints) ;
			UpdateEnergyBar ();
		}
	

		void UpdateEnergyBar ()
		{
			float xValue = -(EnergyHasPercentage() / 2f) - 0.5f;
			energyBar.uvRect = new Rect (xValue, 0f, 0.5f, 1f);
		}

		float EnergyHasPercentage()
		{
			return currentEnergyPoints / maxEnergyPoints;
		}
	}
}
