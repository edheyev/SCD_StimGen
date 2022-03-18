using UnityEngine;
using System.Collections;

public class ResponseBoxGroup : MonoBehaviour {
    /*

    ResponseBox[] boxes;
    public int responseindex=-1; //set to minus one until a response is detected

	// Use this for initialization
	void Start () {
        boxes = gameObject.GetComponentsInChildren<ResponseBox>();
        Debug.Log("boxes: " + boxes.Length);
	}
	
	// Update is called once per frame
	void Update () {
        //prevent more than one response box being active and receiving input

        bool detected = false;
        int active = 0;

        for (int i=0; i<boxes.Length;i++)
        {
            //check whether the box is already indicating a response has been completed
            if (boxes[i].response)
            {
                responseindex = i;
                break;
            }
            //check whether the controller is inside the box
            if (boxes[i].controllerInside)
            {                
                detected = true;
                active = i;
            }

        }
        if (detected)
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                if (i==active)
                {
                    
                    boxes[i].gameObject.SetActive(true);                    
                }
                else
                {                    
                    boxes[i].gameObject.SetActive(false);
                    boxes[i].controllerInside = false; //force
                }
            }
        }
        else
        {
            for (int i = 0; i < boxes.Length; i++)
            {
                boxes[i].gameObject.SetActive(true);
            }
        }
    
    }

    public void ResetResponseBoxGroup()
    {
        for (int i = 0; i < boxes.Length; i++)
        {
            boxes[i].GetComponent<ResponseBox>().ResetResponse(); //make sure each box is not indicating a response              
        }
        responseindex = -1; //reset the response index to -1
    }
    */
}
