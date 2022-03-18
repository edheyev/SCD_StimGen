using UnityEngine;
using System.Collections;

public class ResponseBox : MonoBehaviour
{
    /*

    public bool controllerInside = false;
    //private SteamVR_ControllerManager controllermanager;
    private GameObject lcontrollergo;
    private GameObject rcontrollergo;
   // private SteamVR_TrackedController lcontroller;
  //  private SteamVR_TrackedController rcontroller;
    float holdtimer = 0.0f;
    public float responseholdtime = 1.0f;
    public Light responselight;
    public float flashspeed;
    public bool response = false;
    
    // Use this for initialization
    void Start()
    {
      //  controllermanager = FindObjectOfType<SteamVR_ControllerManager>();
        responselight = gameObject.GetComponentInChildren<Light>();
    }

    // Update is called once per frame
    void Update()
    {
     //   lcontrollergo = controllermanager.left;
    //    rcontrollergo = controllermanager.right;
    //    lcontroller = lcontrollergo.GetComponent<SteamVR_TrackedController>();
      //  rcontroller = rcontrollergo.GetComponent<SteamVR_TrackedController>();
///Debug.Log(lcontroller);
        //Debug.Log(rcontroller);
    }

    void OnTriggerExit(Collider other)
    {
        controllerInside = false;
        responselight.intensity = 1.0f;
    }

    void OnTriggerStay(Collider other)
    {
        controllerInside = true;
        if (Time.time < holdtimer && (other.name == "LControlSphere" || other.name == "RControlSphere"))
        {
            //flash light, vibrate controller until holdtime

            float vibetime = 1500.0f + (Time.time - holdtimer) / responseholdtime * 2000.0f;

            responselight.intensity = Mathf.Cos((Time.time - holdtimer) /responseholdtime * 2.0f * Mathf.PI *flashspeed) /2.0f+0.5f;

            //trigger haptic
            if (other.name == "LControlSphere")
            {
                if (lcontroller.controllerIndex > 0)
                {
                    //Debug.Log(other.name);
                    SteamVR_Controller.Input((int)lcontroller.controllerIndex).TriggerHapticPulse((ushort)vibetime);                    
                }
            }
            else
            {
                if (rcontroller.controllerIndex > 0)
                {
                    //Debug.Log(other.name);
                    SteamVR_Controller.Input((int)rcontroller.controllerIndex).TriggerHapticPulse((ushort)vibetime);

                }
            }
        }
        if (Time.time >= holdtimer && (other.name == "LControlSphere" || other.name == "RControlSphere"))
        {
            //record a response after holdtime
            response = true;          
        }
    }

    void OnTriggerEnter(Collider other)
    {
        controllerInside = true;
        if (other.name == "LControlSphere" || other.name == "RControlSphere")
        {
            //trigger haptic
            if (other.name == "LControlSphere")
            {
                if (lcontroller.controllerIndex > 0)
                {
                    //Debug.Log(other.name);
                  //  SteamVR_Controller.Input((int)lcontroller.controllerIndex).TriggerHapticPulse(500);
                }
            }
            else
            {
                if (rcontroller.controllerIndex > 0)
                {
                    //Debug.Log(other.name);
                   // SteamVR_Controller.Input((int)rcontroller.controllerIndex).TriggerHapticPulse(500);
                }
            }

            //reset hold timer
            //Debug.Log(other.name);
            holdtimer = Time.time + responseholdtime;
        }
    }

    public void ResetResponse()
    {
        response = false;
        controllerInside = false;
    }
}
*/
}