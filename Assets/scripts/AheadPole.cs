using UnityEngine;
using System.Collections;

public class AheadPole : MonoBehaviour {

    GameObject vrcamera;
    GameObject desktopCamera;
    public float offsetdistance = 0;    

	// Use this for initialization
	void Start () {
        /*
        vrcamera = GameObject.Find("Camera (eye)");
        if (vrcamera==null)
        {
            //headset not available
            //SteamVR disabled
            SteamVR_Camera tempcamsteamvr = GameObject.Find("Camera").GetComponent<SteamVR_Camera>();
            if (!(tempcamsteamvr.enabled))
            {
                vrcamera = GameObject.Find("Camera");                
            }
           
        }
        */
        desktopCamera = GameObject.Find("DesktopCamera");

                
    }
	
	// Update is called once per frame
	void Update () {

        //Vector3 newpos = vrcamera.transform.position;
        //Vector3 ahead = vrcamera.transform.eulerAngles;
       // Quaternion currentRotation = Quaternion.Euler(0, ahead.y, 0);
                
        //newpos.y = vrcamera.transform.position.y;

        //transform.position = newpos;
        //transform.rotation = currentRotation;
        


	}
}
