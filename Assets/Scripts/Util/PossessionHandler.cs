using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PossessionHandler : MonoBehaviour
{
    public GameObject possessed;
    
    public Material possessedMat;    

    public bool canPossess = false;

    [SerializeField] GameObject dummyObject;
    [SerializeField] GameObject startingPossession;

    GameObject lookingAt;

    Dictionary<GameObject, Material> trueMaterials = new Dictionary<GameObject, Material>();
    Dictionary<GameObject, Material> trueMaterialLookTo = new Dictionary<GameObject, Material>();

    ObjectStore store;
    PlacementController placement;

    RaycastHit mouseHit;

    [SerializeField] Shader defShader;
    [SerializeField] Shader lookingAtShader;
    [SerializeField] Shader possessedShader;

    Vector3 possessionPoint;

    int layerDefault = 0;
    int ignoreLayer = 2;

    string actualTag;
    string possessedTag = "possessed";
   
    void Awake()
    {
        store = GetComponent<ObjectStore>();
        placement = GetComponent<PlacementController>();

        possessed = dummyObject;
        lookingAt = startingPossession;

        Possess();
    }

    // Update is called once per frame
    void Update()
    {

        //Draws ray to mouse point to highlight possible objects
        if (canPossess)
        {
            DrawRay();
        }
        else
        {
            LookAt(dummyObject);
        }
        
        //TODO if the player is at the end of a tree line >>> draw possible tree
    }

    public void Possess()
    {

        if (lookingAt == dummyObject)
        {
            return;
        }

        //Delete placement (if drawn)
        placement.DestroyPlacement();

        //Reset currently possesed
        possessed.layer = layerDefault;

        foreach (MeshRenderer mR in possessed.GetComponentsInChildren<MeshRenderer>())
        {
            Material mat;

            if (trueMaterials.TryGetValue(mR.gameObject, out mat))
            {
                Material newMat = Material.Instantiate(mat);
                newMat.shader = defShader;

                Destroy(mat);

                mR.material = newMat;

                mR.gameObject.layer = layerDefault;
            }
        }

        //Clear trueMaterials
        trueMaterials.Clear();
        //Set possessed to target and look at dummy obj
        possessed = lookingAt;
        LookAt(dummyObject);
        possessed.layer = ignoreLayer;

        //Setting the newly possessed objects material
        foreach (MeshRenderer mR in possessed.GetComponentsInChildren<MeshRenderer>())
        {
            trueMaterials.Add(mR.gameObject, mR.material);

            Material mat = Instantiate(possessedMat);
            mat.shader = possessedShader;

            mR.material = mat;

            mR.gameObject.layer = ignoreLayer;
        }        

        //Draw a tree if possessing
        if (possessed.CompareTag("tree"))
        {
            HandleTreePossession(possessed);
        }        
    }

    void DrawRay()
    {
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out mouseHit, 100))
        {
            GameObject hitObj = mouseHit.collider.gameObject.transform.root.gameObject;

            if (hitObj.layer == layerDefault)
            {
                LookAt(hitObj);
            }
            else
            {
                LookAt(dummyObject);
            }
        }
        else
        {
            LookAt(dummyObject);
        }
    }    

    void HandleTreePossession(GameObject tree)
    {
        string rootTree = null;

        if (!store.treePattern.IsMatch(tree.name))
        {
            return;
        }

        rootTree = store.treePattern.Match(tree.name) + "_";

        PulseController pC = GameObject.Find(rootTree).GetComponent<PulseController>();

        if (pC.GetLastInLine().name.Equals(tree.name))
        {
            placement.UpdateObjectToPlace(store.GetTreeName(tree.name), "tree", store.GetTree(), mouseHit.point);
            placement.placing = true;
        }
    }

    void LookAt(GameObject objHit)
    {
        //Resets the material on the object the player WAS looking at
        foreach (MeshRenderer mR in lookingAt.GetComponentsInChildren<MeshRenderer>())
        {
            Material newMat = Instantiate(mR.material);
            newMat.shader = defShader;

            Material toDelete = mR.material;

            mR.material = newMat;
            Destroy(toDelete);
        }

        //Sets the looking at object
        lookingAt = objHit;

        if (objHit == dummyObject)
        {
            return;
        }

        //Sets an outline on the object the player is now looking at
        foreach (MeshRenderer mR in objHit.GetComponentsInChildren<MeshRenderer>())
        {
            Material newMat = Instantiate(mR.material);
            newMat.shader = lookingAtShader;

            Material toDelete = mR.material;

            mR.material = newMat;
            Destroy(toDelete);
        }        
    }
}
