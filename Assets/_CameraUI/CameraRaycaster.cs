using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;
using System.Collections.Generic;


namespace RPG.CameraUI
{
	public class CameraRaycaster : MonoBehaviour
	{
		// INSPECTOR PROPERTIES RENDERED BY CUSTOM EDITOR SCRIPT
		[SerializeField] int[] layerPriorities;

		[SerializeField] Texture2D walkCursor = null;   

		[SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

		const int POTENTIALLY_WALKABLE_LAYER = 8;

	    float maxRaycastDepth = 100f; // Hard coded value
		int topPriorityLayerLastFrame = -1; // So get ? from start with Default layer terrain

		// Setup delegates for broadcasting layer changes to other classes
	    public delegate void OnCursorLayerChange(int newLayer); // declare new delegate type
	    public event OnCursorLayerChange notifyLayerChangeObservers; // instantiate an observer set

		public delegate void OnClickPriorityLayer(RaycastHit raycastHit, int layerHit); // declare new delegate type
		public event OnClickPriorityLayer notifyMouseClickObservers; // instantiate an observer set

		public delegate void OnRightClick(RaycastHit raycastHit, int layerHit);
		public event OnRightClick notifyRightClickObservers;

		public delegate void OnMouseOverTerrain(Vector3 destination);
		public event OnMouseOverTerrain notifyMouseOverPotentiallyWalkableObservers;


	    void Update()
		{
			// Check if pointer is over an interactable UI element
			if (EventSystem.current.IsPointerOverGameObject ())
			{
				// implement UI interacition
			}
			else
			{
				Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
				// specify layer priority here
				//if(RaycastForEnemy(ray)) { return; }
				if(RaycastForPotentiallyWalkable(ray)) { return; }
				//TODO remove
				FarTooComplex ();
			}
		}

		private bool RaycastForEnemy(Ray ray)
		{
			throw new KeyNotFoundException();
		}

		private bool RaycastForPotentiallyWalkable(Ray ray)
		{
			RaycastHit hitInfo;
			LayerMask potentiallyWalkableLayer = 1 << POTENTIALLY_WALKABLE_LAYER;
			bool potentiallyWalkableHit = Physics.Raycast (ray, out hitInfo, maxRaycastDepth, potentiallyWalkableLayer);
			if(potentiallyWalkableHit)
			{
				Cursor.SetCursor (walkCursor, cursorHotspot, CursorMode.Auto);
				notifyMouseOverPotentiallyWalkableObservers(hitInfo.point);
				return true;
			}
			return false;
		}

		void FarTooComplex ()
		{
			// Raycast to max depth, every frame as things can move under mouse
			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
			RaycastHit[] raycastHits = Physics.RaycastAll (ray, maxRaycastDepth);
			RaycastHit? priorityHit = FindTopPriorityHit (raycastHits);
			if (!priorityHit.HasValue)// if hit no priority object
			 {
				NotifyObserersIfLayerChanged (0);
				// broadcast default layer
				return;
			}
			// Notify delegates of layer change
			var layerHit = priorityHit.Value.collider.gameObject.layer;
			NotifyObserersIfLayerChanged (layerHit);
			// Notify delegates of highest priority game object under mouse when clicked 
			if (Input.GetMouseButton (0)) {
				notifyMouseClickObservers (priorityHit.Value, layerHit);
			}
			if (Input.GetMouseButtonDown (1)) {
				notifyRightClickObservers (priorityHit.Value, layerHit);
			}
		}

		void NotifyObserersIfLayerChanged(int newLayer)
		{
			if (newLayer != topPriorityLayerLastFrame)
			{
				topPriorityLayerLastFrame = newLayer;
				notifyLayerChangeObservers (newLayer);
			}
		}

		RaycastHit? FindTopPriorityHit (RaycastHit[] raycastHits)
		{
			// Form list of layer numbers hit
			List<int> layersOfHitColliders = new List<int> ();
			foreach (RaycastHit hit in raycastHits)
			{
				layersOfHitColliders.Add (hit.collider.gameObject.layer);
			}

			// Step through layers in order of priority looking for a gameobject with that layer
			foreach (int layer in layerPriorities)
			{
				foreach (RaycastHit hit in raycastHits)
				{
					if (hit.collider.gameObject.layer == layer)
					{
						return hit; // stop looking
					}
				}
			}
			return null; // because cannot use GameObject? nullable
		}
	}
}