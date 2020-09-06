using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnHandler : MonoBehaviour
{
    OfficeController office;

    // Start is called before the first frame update
    void Awake()
    {
        office = transform.parent.gameObject.GetComponent<OfficeController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.name == office.GetLastSpawned().name)
        {
            office.Spawn();
        }
    }
}
