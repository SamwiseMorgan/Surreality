using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PulseController : MonoBehaviour
{
    public List<GameObject> pulseLine = new List<GameObject>();

    public GameObject pulseObj;

    GameObject pulseRef;

    ObjectStore store;

    PulseSplitter splitsAt = null;

    bool pulsing;

    int targetIndex;

    float speed;

    void Awake()
    {
        store = GameObject.Find("MainCamera").GetComponent<ObjectStore>();

        gameObject.layer = 0;

        //Getting lightPoint of this object
        Transform[] transforms = GetComponentsInChildren<Transform>();

        foreach (Transform t in transforms)
        {
            if (t.gameObject.CompareTag("light_point"))
            {
                pulseLine.Add(t.gameObject);
            }
        }

        pulseRef = GameObject.FindGameObjectWithTag("pulse");

        CapsuleCollider cC = gameObject.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider;
        cC.isTrigger = true;
        cC.center = new Vector3(0, 0, 0);
        cC.radius = 2.5f;
        cC.height = 25f;
    }

        // Update is called once per frame
        void Update()
    {        

        if (pulsing)
        {
            if (PulseArrivedAtTarget())
            {
                if (targetIndex >= pulseLine.Count - 1)
                {
                    if (splitsAt != null)
                    {
                        splitsAt.Split();
                    }

                    Destroy(pulseObj);                   
                    pulsing = false;
                    return;
                }
                else
                {
                    targetIndex ++;
                }                
            }


            pulseObj.transform.position = Vector3.MoveTowards(pulseObj.transform.position, pulseLine[targetIndex].transform.position, Time.deltaTime * store.speedOfPulses);
        }

    }

    public void Add(GameObject newTree)
    {
        Transform[] transforms = newTree.GetComponentsInChildren<Transform>();

        PulseSplitter splitter;

        if (newTree.TryGetComponent<PulseSplitter>(out splitter))
        {
            Debug.Log("Spliiter placed");
            splitsAt = splitter;

            foreach (Transform t in transforms)
            {
                if (t.gameObject.CompareTag("light_point") && t.parent.gameObject.name.Equals(splitsAt.gameObject.name))
                {
                    pulseLine.Add(t.gameObject);
                    return;
                }
            }
        }

        foreach (Transform t in transforms)
        {
            if (t.gameObject.CompareTag("light_point"))
            {
                pulseLine.Add(t.gameObject);
            }
        }       
    }

    public void StartPulse(GameObject newPulse)
    {
        if (newPulse == null)
        {
            newPulse = Instantiate(pulseRef, pulseLine[0].transform.position, pulseLine[0].transform.rotation);
        }

        pulseObj = newPulse;

        targetIndex = 0;

        pulsing = true;
    }

    public GameObject GetLastInLine()
    {
        return pulseLine[pulseLine.Count - 1].transform.parent.gameObject;
    }

    bool PulseInstantiated()
    {
        return pulseObj != null;
    }

    bool PulseArrivedAtTarget()
    {
        float dist = Vector3.Distance(pulseObj.transform.position, pulseLine[targetIndex].transform.position);

        return dist < 1;
    }   
}
