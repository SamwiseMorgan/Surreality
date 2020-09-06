using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LocationHandler : MonoBehaviour
{
    public ObjectStore store;
    public PlacementController placement;
    public PossessionHandler possession;
    public PulseController pulseController;

    public float checkDistance = 8f;

    List<Collider> colliders = new List<Collider>();

    SphereCollider sphereCollider;

    void Awake()
    {
        //Lights on the object are set up here
        foreach (Light tempLight in GetComponentsInChildren<Light>())
        {
            tempLight.renderMode = LightRenderMode.ForcePixel;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float dist = Vector3.Distance(possession.possessed.transform.position, transform.position);

        if (dist <= checkDistance)
        {
            placement.canPlace = true;
            placement.Redraw();
        }
        else
        {
            placement.canPlace = false;
            placement.Redraw();
        }
    }
}
