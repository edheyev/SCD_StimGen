using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class texty : MonoBehaviour {

    public GameObject testob;
    Renderer rend;

    public Color red;

    public Color blue;

	// Use this for initialization
	void Start () {
        rend = testob.GetComponent<Renderer>();
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.R))
        {
            rend.material.SetColor("_OutlineColor", red);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            rend.material.SetColor("_OutlineColor", blue);
        }
    }
}
