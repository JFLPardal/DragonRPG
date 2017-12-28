using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRaycaster))]
public class CursorAffordance : MonoBehaviour {

	[SerializeField] Texture2D walkCursor = null;
	[SerializeField] Texture2D targetCursor = null;
	[SerializeField] Texture2D unknownCursor = null;
	[SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);
	[SerializeField] const int layerWalkable = 8;
	[SerializeField] const int layerEnemy = 9;

	CameraRaycaster cameraRaycaster;

	void Start () {
		cameraRaycaster = GetComponent<CameraRaycaster> ();
		cameraRaycaster.notifyLayerChangeObservers += OnLayerChanged;
	}

	void OnLayerChanged (int newLayer) //called only when the raycast is hitting a different layer than it was
	{
		switch(newLayer)
		{
		case(layerWalkable):
				Cursor.SetCursor (walkCursor, cursorHotspot, CursorMode.Auto);
				break;
		case(layerEnemy):
				Cursor.SetCursor (targetCursor, cursorHotspot, CursorMode.Auto);
				break;
			default:
				Cursor.SetCursor (unknownCursor, cursorHotspot, CursorMode.Auto);
				return;
		}
	}
}

//TODO consider de-registering OnLayerChanged on leaving all game scenes
