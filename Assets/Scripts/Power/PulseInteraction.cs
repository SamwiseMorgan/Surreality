using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseInteraction : MonoBehaviour
{

    HostileController hostile;
    
    void Awake()
    {
        hostile = GameObject.Find("Controller").GetComponent<HostileController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("crawler"))
        {
            hostile.Petrify(other.gameObject);
        }
    }
}
