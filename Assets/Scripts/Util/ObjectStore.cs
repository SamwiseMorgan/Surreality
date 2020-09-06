using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class ObjectStore : MonoBehaviour
{
    public Dictionary<GameObject, int> resourceAndProbability;

    //Last controlled object references
    public GameObject lastControlled;
    public GameObject lastControlledTree;

    public int clockSpawnedAt;

    //Tree for route pattern
    public Regex treePattern = new Regex(@"^TreeRoute\w+(?=_)");

    //Speed of pulses
    public float speedOfPulses;
    //Tree group of tree being placed
    public string treeGroup;

    //Lists for active objects
    List<GameObject> allActiveObjects = new List<GameObject>();
    List<GameObject> activeTrees = new List<GameObject>();
    List<GameObject> activeResources = new List<GameObject>();

    //Populated from resource pool
    GameObject[] referenceTrees;
    GameObject[] resourcePool;
    //ObjectsToPlace
    GameObject clock;
    //Hostile AI
    GameObject crawler;

    //Dictionary of PulseController(s) to get by name
    Dictionary<string, PulseController> pulseControllers = new Dictionary<string, PulseController>();

    [SerializeField] GameObject spawnPointParent;

    //TODO Paths to objects
    string pathToCrawler = "Prefabs/Hostiles/P_Crawler";
    string pathToClock = "Prefabs/Powered/P_Clock";
    string pathToResourcePool = "Prefabs/ResourcePool";
    string pathToTrees = "Prefabs/Trees";


    // Start is called before the first frame update
    void Awake()
    {

        //Loading in from resources
        //Arrays
        resourcePool = Resources.LoadAll(pathToResourcePool).Cast<GameObject>().ToArray();
        referenceTrees = Resources.LoadAll(pathToTrees).Cast<GameObject>().ToArray();
        //Pieces to place
        clock = Resources.Load(pathToClock) as GameObject;
        //Hostial AI
        crawler = Resources.Load(pathToCrawler) as GameObject;

        Debug.Log(resourcePool.Length + referenceTrees.Length);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject GetTree()
    {
        if (activeTrees.Count == clockSpawnedAt)
        {
            //instantiate
            return clock;
        } 

        return referenceTrees[Random.Range(0, referenceTrees.Length)];        
    }

    public void Store(GameObject toStore)
    {
        //Add to store
        allActiveObjects.Add(toStore);
        
        //Add tree to store
        if (toStore.CompareTag("tree"))
        {
            activeTrees.Add(toStore);
            UpdatePulseControllers(toStore);
        }
    }

    public void StoreResource(GameObject objToStore)
    {
        activeResources.Add(objToStore);
    }

    public void UpdatePulseControllers(GameObject obj) 
    {
        //If the object does not match the correct parrent then we need to ignore it
        if (!treePattern.IsMatch(obj.name))
        {
            Debug.LogWarning(obj.name + ": Name does not match the format for a tree.");
            return;
        }

        Debug.Log("Adding " + obj.name + " to " + treePattern.Match(obj.name).Value + "_");
        //Get the PulseController from the origin tree
        PulseController pC = GameObject.Find(treePattern.Match(obj.name).Value + "_").GetComponent<PulseController>();
        pC.Add(obj);
    }

    public GameObject[] GetResources()
    {
        return resourcePool;
    }

    public string GetTreeName(string refName) 
    {

        if (treePattern.IsMatch(refName))
        {
            treeGroup = treePattern.Match(refName).Value;

            return treeGroup + "_" + (activeTrees.Count + 1);
        }

        return null;
    }

    public string MatchTreeGroup(string toMatch)
    {
        if (treePattern.IsMatch(toMatch))
        {
            return treePattern.Match(toMatch).Value;
        }

        return null;
    }

    public GameObject GetCrawler()
    {
        return crawler;
    }
}
