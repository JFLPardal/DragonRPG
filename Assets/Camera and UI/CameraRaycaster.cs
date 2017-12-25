using UnityEngine;

public class CameraRaycaster : MonoBehaviour
{
    public Layer[] layerPriorities = {
        Layer.enemy,
        Layer.walkable
    };

	[SerializeField] float distanceToBackground = 100f;
    Camera viewCamera;

    RaycastHit raycastHit;
    public RaycastHit hit
    {
		get { return raycastHit; }
    }

    Layer layerHit;
    public Layer currentLayerHit
    {
        get { return layerHit; }
    }

	public delegate void OnLayerChange(Layer newLayer); //declare new delegate type
	public event OnLayerChange layerChangeObservers; //instantiate an observer set

    void Start() 
    {
        viewCamera = Camera.main;	
    }

    void Update()
    {
        // Look for and return priority layer hit
        foreach (Layer layer in layerPriorities)
        {
            var hit = RaycastForLayer(layer);
            if (hit.HasValue)
			{
				raycastHit = hit.Value;
				if (layerHit != layer) {	//if the new layer is different from the previous, notify all interested
					layerHit = layer;
					layerChangeObservers (layer);
				} else
					layerHit = layer;
                return;
            }
        }

        // Otherwise return background hit
        raycastHit.distance = distanceToBackground;
        layerHit = Layer.raycastEndStop;
    }

    RaycastHit? RaycastForLayer(Layer layer) //question marks let's us return null when nothing is hit
    {
        int layerMask = 1 << (int)layer; // See Unity docs for mask formation
        Ray ray = viewCamera.ScreenPointToRay(Input.mousePosition);

        RaycastHit hit; // used as an out parameter
        bool hasHit = Physics.Raycast(ray, out hit, distanceToBackground, layerMask);
        if (hasHit)
        {
            return hit;
        }
        return null;
    }
}
