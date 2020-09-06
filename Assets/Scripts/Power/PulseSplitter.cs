using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseSplitter : MonoBehaviour
{
    PulseController[] pControllers;

    GameObject comingFrom;

    ObjectStore store;

    PulseController priorController;

    //Called when object is enabled
    private void OnEnable()
    {
        pControllers = GetComponentsInChildren<PulseController>();        

        store = GameObject.Find("Controller").GetComponent<ObjectStore>();

        comingFrom = store.lastControlledTree;

        if (store.treePattern.IsMatch(comingFrom.name))
        {
            GameObject temp = GameObject.Find(store.treePattern.Match(comingFrom.name).Value + "_");
            priorController = temp.GetComponent<PulseController>(); 
        }
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Split()
    {
        foreach (PulseController temp in pControllers)
        {
            GameObject newPulse = Instantiate(priorController.pulseObj, 
                priorController.pulseObj.transform.position,
                priorController.pulseObj.transform.rotation);

            temp.StartPulse(newPulse);
        }
    }

    public void Place()
    {
        foreach (PulseController pC in pControllers)
        {
            pC.transform.parent = null;
            pC.gameObject.layer = 0;
        }

        Destroy(GetComponent<CapsuleCollider>());
    }


}
