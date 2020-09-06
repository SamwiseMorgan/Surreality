using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementController : MonoBehaviour
{
    public GameObject dummyObject;
    public GameObject objectToPlace;

    public Material canPlaceMat;
    public Material cannotPlaceMat;

    public Vector3 hiddenLocation;

    //if this is true placement is possible
    public bool placing = false;
    public bool canPlace = false;

    Dictionary<string, Material> childMaterials = new Dictionary<string, Material>();
    
    Material trueObjectMaterial;

    ObjectStore store;

    bool childrenHaveMeshes = false;

    
    
    void Awake()
    {
        store = GetComponent<ObjectStore>();
    }

    // Update is called once per frame
    void Update()
    {
        if (placing)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {                               
                objectToPlace.transform.position = hit.point;
            }
        }
    }

    public GameObject UpdateObjectToPlace(string newName, string newTag, GameObject refObject, Vector3 targetPos)
    {
        childrenHaveMeshes = false;

        //Create new object & name it
        objectToPlace = Instantiate(refObject as GameObject);
        objectToPlace.transform.position = targetPos;
        objectToPlace.transform.rotation = new Quaternion();
        objectToPlace.name = newName;
        objectToPlace.tag = newTag;

        //Storing mesh material
        MeshRenderer mesh;

        if (objectToPlace.TryGetComponent<MeshRenderer>(out mesh))
        {
            trueObjectMaterial = mesh.material;
        }
        else
        {
            trueObjectMaterial = null;
        }

        foreach(Transform childT in objectToPlace.GetComponentsInChildren<Transform>())
        {
            
            MeshRenderer childMesh;

            if (childT.gameObject.TryGetComponent<MeshRenderer>(out childMesh))
            {               
                if (childMaterials.ContainsKey(childMesh.gameObject.name))
                {
                    continue; 
                }

                childMaterials.Add(childMesh.gameObject.name, childMesh.material);

                childrenHaveMeshes = true;
            }

        }

        //Collider used in placement checking
        CapsuleCollider cC = objectToPlace.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
        cC.isTrigger = true;
        cC.center = new Vector3(0, 0, 0);
        cC.radius = 2.5f;
        cC.height = 25f;

        //Attaches LocationHandler to object >> Handles collision checking
        LocationHandler lH = objectToPlace.AddComponent(typeof(LocationHandler)) as LocationHandler;
        lH.placement = this;
        lH.store = store;
        lH.possession = GetComponent<PossessionHandler>();

        //Case specific construction implementation 
        if (objectToPlace.CompareTag("tree"))
        {
            store.lastControlled = objectToPlace;
            store.lastControlledTree = objectToPlace;

            lH.pulseController = GameObject
                .Find(store.treePattern.Match(objectToPlace.name).Value + "_")
                .GetComponent<PulseController>();

            foreach (GameObject point in lH.pulseController.pulseLine)
            {
                point.transform.parent.gameObject.layer = 2;
            }
        } 
        else
        {
            store.lastControlled = objectToPlace;

            lH.pulseController = null;
        }



        //Putting on ignore raycast layer to prevent hopping
        UpdateLayer(objectToPlace, 2);

        return objectToPlace;
        
    }

    public GameObject Place()
    {
        placing = false;

        //Set objectToPlace back to dummy object & get local reference 
        GameObject placedObject = objectToPlace;
        objectToPlace = dummyObject;

        //Reset Material
        if (trueObjectMaterial != null)
        {
            placedObject.GetComponent<MeshRenderer>().material = trueObjectMaterial;
        }

        foreach (GameObject point in placedObject.GetComponent<LocationHandler>().pulseController.pulseLine)
        {
            point.transform.parent.gameObject.layer = 0;
        }

        //Remove Location handler
        Destroy(placedObject.GetComponent<LocationHandler>());


        if (childrenHaveMeshes)
        {
            UpdateChildMaterials(placedObject, true);
        }        

        PulseSplitter splitter;

        if (placedObject.TryGetComponent(out splitter))
        {
            splitter.Place();
        }

        UpdateLayer(placedObject, 0);

        return placedObject;
    }

    public void Redraw()
    {
        Material tempMat = canPlace ? canPlaceMat : cannotPlaceMat;
        MeshRenderer tempMesh;

        if (objectToPlace.TryGetComponent<MeshRenderer>(out tempMesh))
        {
            tempMesh.material = tempMat;
        }

        if (childrenHaveMeshes)
        {
            UpdateChildMaterials(objectToPlace, false);
        }
    }

    public void UpdateChildMaterials(GameObject refObj, bool place)
    {
        Material tempMat;
        //If we are placing
        if (place)
        {           
            foreach (MeshRenderer childMesh in refObj.GetComponentsInChildren<MeshRenderer>())
            {
                //If we have a texture for it, give it back 
                if (childMaterials.TryGetValue(childMesh.gameObject.name, out tempMat))
                {                    
                    childMesh.material = tempMat;
                }
            }

            childMaterials.Clear();

        }
        else
        {
            //Sets the materials to guide materials
            tempMat = canPlace ? canPlaceMat : cannotPlaceMat;

            foreach (MeshRenderer childMesh in refObj.GetComponentsInChildren<MeshRenderer>())
            {
                if (childMaterials.ContainsKey(childMesh.gameObject.name))
                {
                    childMesh.material = tempMat;
                }
            }
        }
    }

    public void DestroyPlacement()
    {
        if (objectToPlace == dummyObject)
        {
            return;
        }
        else
        {
            Destroy(objectToPlace);
            objectToPlace = dummyObject;
        }
    }

    float GetColliderRadius()
    {
        switch (tag)
        {
            case "tree":
                return 5f;

            default:
                return 5f;
        }
    }

    void UpdateLayer(GameObject toUpdate, int layerIndex)
    {
        toUpdate.layer = layerIndex;

        foreach (Transform t in toUpdate.GetComponentsInChildren<Transform>())
        {
            t.gameObject.layer = layerIndex;
        }
    }
}
