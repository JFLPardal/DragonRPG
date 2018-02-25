using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using RPG.Characters;	// so we can detect by type

namespace RPG.CameraUI
{
	public class CameraRaycaster : MonoBehaviour
	{
		[SerializeField] Texture2D walkCursor = null;   
		[SerializeField] Texture2D enemyCursor = null;   
		[SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

		const int POTENTIALLY_WALKABLE_LAYER = 8;

	    float maxRaycastDepth = 100f; // Hard coded value

		public delegate void OnMouseOverTerrain(Vector3 destination);
		public event OnMouseOverTerrain notifyMouseOverPotentiallyWalkableObservers;

		public delegate void OnMouseOverEnemy(Enemy enemy);
		public event OnMouseOverEnemy notifyMouseOverEnemyObservers;	

	    void Update()
		{
			// Check if pointer is over an interactable UI element
			if (EventSystem.current.IsPointerOverGameObject ())
			{
				// implement UI interacition
			}
			else
			{
				PerformRaycasts ();
			}
		}

		void PerformRaycasts()
		{
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			// specify layer priority here, order matters
			if(RaycastForEnemy(ray)) { return; }
			if(RaycastForPotentiallyWalkable(ray)) { return; }
		}

		private bool RaycastForEnemy(Ray ray)
		{
			RaycastHit hitInfo;
			Physics.Raycast (ray, out hitInfo, maxRaycastDepth);
			var gameObjectHit = hitInfo.collider.gameObject;
			var enemyHit = gameObjectHit.GetComponent<Enemy>();
			if(enemyHit != null)
			{
				Cursor.SetCursor (enemyCursor, cursorHotspot, CursorMode.Auto);
				notifyMouseOverEnemyObservers (enemyHit);
				return true;
			}
			return false;
		}

		private bool RaycastForPotentiallyWalkable(Ray ray)
		{
			RaycastHit hitInfo;
			LayerMask potentiallyWalkableLayer = 1 << POTENTIALLY_WALKABLE_LAYER;
			bool potentiallyWalkableHit = Physics.Raycast (ray, out hitInfo, maxRaycastDepth, potentiallyWalkableLayer);
			if(potentiallyWalkableHit)
			{
				Cursor.SetCursor (walkCursor, cursorHotspot, CursorMode.Auto);
				//notifyMouseOverPotentiallyWalkableObservers(hitInfo.point);
				return true;
			}
			return false;
		}
	}
}