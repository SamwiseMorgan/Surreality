using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    public PulseController[] startingControllers;

    public int speedOfMouse;


    List<PulseController> pulseControllers = new List<PulseController>();

    HostileController hostile;
    ObjectStore store;
    OfficeController office;
    PlacementController placement;
    PossessionHandler possession;

    float screenCentreH;
    float screenCentreW;

    void Awake()
    {
        hostile = GetComponent<HostileController>();
        store = GetComponent<ObjectStore>();
        placement = GetComponent<PlacementController>();
        possession = GetComponent<PossessionHandler>();
        office = GameObject.Find("LeMinotaure").GetComponent<OfficeController>();

        foreach (PulseController pC in startingControllers)
        {
            pulseControllers.Add(pC);
        }

        screenCentreH = Screen.height / 2;
        screenCentreW = Screen.width / 2;
    }

    // Update is called once per frame
    void Update()
    {
        //Finding angle between object and mouse pos
        
        if (Input.GetKey(KeyCode.Space))
        {
            possession.canPossess = true;
        }
        else
        {
            possession.canPossess = false;
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            possession.Possess();
        }


        //Handling click
        if (Input.GetMouseButtonDown(0))
        {
            
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                //Grabbing tree to place
                if (hit.collider.gameObject.CompareTag("tree"))
                {
                    Debug.Log("Hit >> " + hit.collider.gameObject.name);

                    string newTreeName = store.GetTreeName(hit.collider.gameObject.name);

                    if (newTreeName == null)
                    {
                        Debug.LogWarning(hit.collider.gameObject.name + " was tagged with 'tree' but did not match pattern");
                        return;
                    }

                    placement.UpdateObjectToPlace(newTreeName, "tree", store.GetTree(), hit.point);
                    placement.placing = true;
                    return;
                }

                //TODO REMOVE USED FOR TESTING NAV
                if (hit.collider.CompareTag("floor"))
                {
                    hostile.AllGoTo(hit.point);
                }

                //Placing object
                if (placement.placing && placement.canPlace)
                {
                    //Places object and stores it
                    store.Store(placement.Place());
                }

            }

        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            foreach (PulseController pC in pulseControllers)
            {
                pC.StartPulse(null);
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            office.ToggleSpawn();
        }

    }
}
