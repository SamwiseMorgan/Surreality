using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OfficeController : MonoBehaviour
{
    List<GameObject> activeCrawlers = new List<GameObject>();

    GameObject dummy;
    GameObject spawnPoint;

    ObjectStore store;
    PossessionHandler possession;

    bool spawning = false;

    int maxSpawn = 1;
    
    void Awake()
    {
        GameObject controller = GameObject.Find("Controller");
        store = controller.GetComponent<ObjectStore>();
        possession = controller.GetComponent<PossessionHandler>();

        dummy = GameObject.Find("DummyObject");

        spawnPoint = GetComponentInChildren<SpawnHandler>().gameObject;
    }

    void Update()
    {
        
    }

    public bool ToggleSpawn()
    {
        spawning = !spawning;

        if (spawning)
        {
            Spawn();
        }

        return spawning;
    }

    public List<GameObject> GetActiveCrawlers()
    {
        return activeCrawlers;
    }

    public GameObject GetLastSpawned()
    {
        if (activeCrawlers.Count == 0)
        {
            return dummy;
        }

        return activeCrawlers[activeCrawlers.Count - 1];
    }

    public void UpdateDestinations(Vector3 target)
    {
        foreach (GameObject crawler in activeCrawlers)
        {
            crawler.GetComponent<NavMeshAgent>()
                .SetDestination(target);
        }
    }

    public void Spawn()
    {
        Vector3 spawnVector = new Vector3(
            spawnPoint.transform.position.x,
            spawnPoint.transform.position.y,
            spawnPoint.transform.position.z);

        GameObject spawned = Instantiate(store.GetCrawler() as GameObject, 
            spawnVector, spawnPoint.transform.rotation);

        if (spawned == null)
        {
            Debug.LogError("Crawler Not Spawned.");
            return;
        }

        spawned.name = "Crawler_" + activeCrawlers.Count;

        activeCrawlers.Add(spawned);
        spawned.GetComponent<NavMeshAgent>()
            .SetDestination(GetCrawlerTarget());
    }

    Vector3 GetCrawlerTarget()
    {

        Vector3 target = possession.possessed.transform.position;

        int count = activeCrawlers.Count == 0 ? 4 : activeCrawlers.Count % 4;

        switch (count)
        {
            case 0:
                target.x = target.x - 1.5f;
                break;
            case 1:
                target.x = target.x + 1.5f;
                break;
            case 2:
                target.x = target.y + 1.5f;
                break;
            case 3:
                target.x = target.y - 1.5f;
                break;
        }

        return target;
    }    

   
}
