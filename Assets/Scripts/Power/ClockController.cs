using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClockController : MonoBehaviour
{
    public Button pullButton;
    public Button pushButton;

    GameObject mainCam;
    GameObject hourHand;
    GameObject minuteHand;

    ObjectStore store;

    Transform pullRefernce;
    Transform pushReference;

    Vector3 direction;

    bool movingClockwise = false;
    bool movingCounterClockwise = false;

    float handSpeed = 20;
    float lastAngle;
    
    void Awake()
    {

        pullRefernce = GameObject.Find("ClockPullReference").GetComponent<Transform>();
        pushReference = GameObject.Find("ClockPushReference").GetComponent<Transform>();

        mainCam = GameObject.FindGameObjectWithTag("MainCamera");
        hourHand = GameObject.FindGameObjectWithTag("hour_hand");
        minuteHand = GameObject.FindGameObjectWithTag("minute_hand");

        store = GameObject.Find("Controller").GetComponent<ObjectStore>();

        lastAngle = hourHand.transform.rotation.y;
    }

    // Update is called once per frame
    void Update()
    {

        if (movingClockwise)
        {
            minuteHand.transform.RotateAround(transform.position, minuteHand.transform.up, handSpeed * Time.deltaTime);
            hourHand.transform.RotateAround(transform.position, hourHand.transform.up, (handSpeed / 12) * Time.deltaTime);
            
          
        }
        else if (movingCounterClockwise)
        {
            minuteHand.transform.RotateAround(transform.position, minuteHand.transform.up, -handSpeed * Time.deltaTime);
            hourHand.transform.RotateAround(transform.position, hourHand.transform.up, (-handSpeed / 12) * Time.deltaTime);
        }
    }

    public void Place()
    {
        /**
        pullButton.gameObject.SetActive(true);
        pushButton.gameObject.SetActive(true);

        //Adding Listeners to push button
        EventTrigger pushTriggers = pushButton.gameObject.AddComponent<EventTrigger>();
        //pointer down
        var pushPointerDown = new EventTrigger.Entry();
        pushPointerDown.eventID = EventTriggerType.PointerDown;
        pushPointerDown.callback.AddListener((e) => ToggleClockwise());
        pushTriggers.triggers.Add(pushPointerDown);
        //pointer up
        var pushPointerUp = new EventTrigger.Entry();
        pushPointerUp.eventID = EventTriggerType.PointerUp;
        pushPointerUp.callback.AddListener((e) => ToggleClockwise());
        pushTriggers.triggers.Add(pushPointerUp);

        //Adding Listeners to pull button
        EventTrigger pullTriggers = pullButton.gameObject.AddComponent<EventTrigger>();
        //pointer down
        var pullPointerDown = new EventTrigger.Entry();
        pullPointerDown.eventID = EventTriggerType.PointerDown;
        pullPointerDown.callback.AddListener((e) => ToggleCounterClockwise());
        pullTriggers.triggers.Add(pullPointerDown);
        //pointer up
        var pullPointerUp = new EventTrigger.Entry();
        pullPointerUp.eventID = EventTriggerType.PointerUp;
        pullPointerUp.callback.AddListener((e) => ToggleCounterClockwise());
        pullTriggers.triggers.Add(pullPointerUp);
    **/

    }

    public void ToggleClockwise()
    {
        movingClockwise = !movingClockwise;
    }

    public void ToggleCounterClockwise()
    {
        movingCounterClockwise = !movingCounterClockwise;
    }

    void RotateToCamera()
    {
        direction = mainCam.transform.position - transform.position;
        Quaternion toRotation = Quaternion.FromToRotation(transform.up, direction);
        transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, 1f * Time.time);
    }

    void UpdateSpeedOfPulses()
    {
        float changeInAngle = hourHand.transform.rotation.y - lastAngle;
        float deltaSpeed = 1 / 360;
        deltaSpeed = deltaSpeed * changeInAngle;
        store.speedOfPulses = store.speedOfPulses + deltaSpeed;

        lastAngle = hourHand.transform.rotation.y;
    }
}
