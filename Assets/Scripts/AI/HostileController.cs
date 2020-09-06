using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HostileController : MonoBehaviour
{   
    public GameObject crawler;

    List<GameObject> activeAgents = new List<GameObject>();

    ObjectStore store;

    int rawMaterialWeight;

    void Awake()
    {
        store = GetComponent<ObjectStore>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.H))
        {
            Spawn(crawler);
        }
    }

    public void Petrify(GameObject toPetrify)
    {
        GameObject resourceRef = GetResource();

        if (resourceRef == null)
        {
            Destroy(toPetrify);
            return;
        }

        toPetrify.SetActive(false);

        GameObject resource = Instantiate(resourceRef, 
            toPetrify.transform.position, 
            toPetrify.transform.rotation);

        Destroy(toPetrify);

        store.StoreResource(resource);
    }

    GameObject GetResource()
    {       
        int[] resourceProbs = new int[store.resourceAndProbability.Count];
        int total = 0;
        int index = 0;

        foreach(KeyValuePair<GameObject, int> entry in store.resourceAndProbability)
        {
            resourceProbs[index++] = entry.Value;
            total += total + entry.Value;
        }

        int val = Random.Range(0, total);

        foreach(KeyValuePair<GameObject, int> pair in store.resourceAndProbability)
        {
            if (val <= total)
            {
                return pair.Key;
            }
        }

        return null;
    }

    void Spawn(GameObject toSpawn)
    {
        /**
        int spawnIndex = Random.Range(0, store.GetSpawnPoints().Count);

        GameObject newSpawn = Instantiate(toSpawn,
            store.GetSpawnPoints()[spawnIndex].transform.position,
            store.GetSpawnPoints()[spawnIndex].transform.rotation);

        newSpawn.SetActive(true);

        activeAgents.Add(newSpawn);
    **/
    }

    public void AllGoTo(Vector3 target) 
    {
        foreach (GameObject agentObj in activeAgents)
        {
            NavMeshAgent agent;

            if (agentObj.TryGetComponent<NavMeshAgent>(out agent))
            {
                agent.SetDestination(target);
            }
        }
    }
}
