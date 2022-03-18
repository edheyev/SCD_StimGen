using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelect : MonoBehaviour {
    public GameObject selectedObject;
    Camera cam;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {


        cam = Camera.main;
            RaycastHit hit;
            //Ray ray = Camera.current.ScreenPointToRay(Input.mousePosition);

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.transform.name);
                }
            }
        
    }
    

