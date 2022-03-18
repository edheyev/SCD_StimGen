using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHighlighter : MonoBehaviour {

    PRandStream internal_rng;
    SCDSExperimentController experimentController;
    public PrefabArrayGenerator pag;
    public GameObject targetObject;
    public Color color1;
    public Color color2;
    [Range(1.0f, 5.0f)]
    public float outlineWidth;
    // Use this for initialization
    void Start () {
         internal_rng = new PRandStream();
        pag = GameObject.Find("PrefabArrayGenerator").GetComponent<PrefabArrayGenerator>();
        experimentController = GameObject.Find("ExperimentController").GetComponent<SCDSExperimentController>();

    }

    // Update is called once per frame
    void Update () {
		
	}


    public void HighlightTwo(TabletopElement target, List<TabletopElement> arraySpec)
    {
        Renderer targetRenderer = target.prefabtype.GetComponent<Renderer>(); //get target shader ready

        int foilIndex = internal_rng.RangeWithSkip(0, arraySpec.Count, pag.indextomove); //pick foild excluding target
        Debug.Log(foilIndex);
        TabletopElement foil = arraySpec[foilIndex]; 
        Renderer foilRenderer = foil.prefabtype.GetComponent<Renderer>(); //get foil shader ready

        //randomize colourpick!!!

        experimentController.targetElement = target;

        experimentController.foilElement = foil;


        targetRenderer.sharedMaterial.SetColor("_OutlineColor", color1); //set outline colour and size
        targetRenderer.sharedMaterial.SetFloat("_OutlineWidth", outlineWidth);
        foilRenderer.sharedMaterial.SetColor("_OutlineColor", color1);
        foilRenderer.sharedMaterial.SetFloat("_OutlineWidth", outlineWidth);
    }





    public TabletopElement HighlightTwoAndOcclude(TabletopElement target, List<TabletopElement> arraySpec)
    { 
        Renderer targetRenderer = target.prefabtype.GetComponent<Renderer>(); //get target shader ready

        int foilIndex = internal_rng.RangeWithSkip(0, arraySpec.Count, pag.indextomove); //pick foild excluding target
        Debug.Log(foilIndex);
        TabletopElement foil = arraySpec[foilIndex];
        Renderer foilRenderer = foil.prefabtype.GetComponent<Renderer>(); //get foil shader ready

        for (int i = 0; i < arraySpec.Count; i++)
        {
            if(i != foilIndex && i != pag.indextomove)
            {
                //arraySpec[i].prefabtype.gameObject.SetActive(false);
            }
        }
        for (int i = 0; i < arraySpec.Count; i++)
        {
            arraySpec[i].prefabtype.SetActive(false);
            if (i == foilIndex || i == pag.indextomove)
            {
            }
            else
            {
                arraySpec[i].prefabtype.SetActive(false);

            }
        }
        //randomize colourpick!!!

        targetRenderer.sharedMaterial.SetColor("_OutlineColor", color1); //set outline colour and size
        targetRenderer.sharedMaterial.SetFloat("_OutlineWidth", outlineWidth);
        foilRenderer.sharedMaterial.SetColor("_OutlineColor", color2);
        foilRenderer.sharedMaterial.SetFloat("_OutlineWidth", outlineWidth);
        return foil;
    }


    public void HighlightTarget(TabletopElement target, Color color)
    {
        Renderer targetRenderer = target.prefabtype.GetComponent<Renderer>(); //get target shader ready
        targetRenderer.sharedMaterial.SetColor("_OutlineColor", color); //set outline colour and size
        targetRenderer.sharedMaterial.SetFloat("_OutlineWidth", outlineWidth);

    }

    public void ResetObjects(List<TabletopElement> arraySpec)
    {
        for (int i = 0; i<arraySpec.Count; i++)
        {
            //arraySpec[i].prefabtype.gameObject.SetActive(true);
            Renderer thisObjRenderer = arraySpec[i].prefabtype.GetComponent<Renderer>();
            thisObjRenderer.sharedMaterial.SetFloat("_OutlineWidth", 1.0f);
            

        }
    }

}
