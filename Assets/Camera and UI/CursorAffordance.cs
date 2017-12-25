using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CameraRaycaster))]
public class CursorAffordance : MonoBehaviour {

	[SerializeField] Texture2D walkCursor = null;
	[SerializeField] Texture2D targetCursor = null;
	[SerializeField] Texture2D unknownCursor = null;
	[SerializeField] Vector2 cursorHotspot = new Vector2(0, 0);

	CameraRaycaster cameraRaycaster;

	void Start () {
		cameraRaycaster = GetComponent<CameraRaycaster> ();
		cameraRaycaster.layerChangeObservers += OnLayerChanged;
	}

	void OnLayerChanged (Layer newLayer) //called only when the raycast is hitting a different layer than it was
	{
		switch(newLayer)
		{
			case(Layer.walkable):
				Cursor.SetCursor (walkCursor, cursorHotspot, CursorMode.Auto);
				break;
			case(Layer.enemy):
				Cursor.SetCursor (targetCursor, cursorHotspot, CursorMode.Auto);
				break;
			case(Layer.raycastEndStop):
				Cursor.SetCursor (unknownCursor, cursorHotspot, CursorMode.Auto);
				break;
			default:
				Debug.LogError ("Don't know what cursor to show");
				return;
		}
	}
}

//TODO consider de-registering OnLayerChanged on leaving all game scenes
