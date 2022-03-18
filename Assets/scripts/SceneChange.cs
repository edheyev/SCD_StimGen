using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChange : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnMouseClick()
    {
        Application.LoadLevel("VCE_Testscene");
    }

    public void OnPractiseButtonClick()
    {
        Application.LoadLevel("VCE_Tutorial");
    }
}
