using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConditionsController : MonoBehaviour {

    //this script is worth keeping around as I might put in room conditions eg lighting etc this could be done here

        /*
    //public List<MountainConditions> conditions; //set up the available conditions in the Unity Editor
    //public UltimateSky.UltimateSky sky;
    //public Terrain terrain;
    //public int currentconditions;

    public Material cloudmaterial;
    RenderTexture skyrender ;
    Texture2D dummy;    

    // Use this for initialization
    void Start () {
        dummy = new Texture2D(512, 512, TextureFormat.ARGB32, false);
        SetConditions(0);
        CloudEngine cloudengine = GameObject.Find("CloudEngine").GetComponent<CloudEngine>();
        skyrender = cloudengine.rendertexture;        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown("."))
        {
            currentconditions++;
            if (currentconditions>=conditions.Count)
            {
                currentconditions = 0;
            }
            SetConditions(currentconditions);          
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            cloudmaterial.SetFloat("Seed", Random.value);
            Graphics.Blit(dummy,skyrender,cloudmaterial);
            RenderTexture.active = skyrender;
            Texture2D cloudstemp = new Texture2D(512, 512, TextureFormat.ARGB32, false);
            cloudstemp.ReadPixels(new Rect(512, 512, 512, 512),0,0);
            cloudstemp.Apply();
            RenderTexture.active = null;
            sky.cloudTexture2 = cloudstemp;
            sky.cloudTexture1 = cloudstemp;
        }

    }

    public void SetConditions(int index)
    {
        if (index<conditions.Count)
        {

            System.DateTime datetime = System.DateTime.Parse(conditions[index].datetime);            
            sky.calendar.yearMonthDay = new Vector3(datetime.Year, datetime.Month, datetime.Day);
            sky.calendar.hourMinSec = new Vector3(datetime.Hour, datetime.Minute, datetime.Second);

            SplatPrototype[] oldProtos = terrain.terrainData.splatPrototypes;
            SplatPrototype[] newProtos = new SplatPrototype[oldProtos.Length];

            newProtos[0] = oldProtos[0];

            newProtos[0].texture = (Texture2D)conditions[index].material.GetTexture("_MainTex");
            newProtos[0].normalMap = (Texture2D)conditions[index].material.GetTexture("_BumpMap");
            terrain.terrainData.splatPrototypes = newProtos;

            RenderSettings.fogDensity = conditions[index].fogdensity;
            sky.cloudFadeIn    = conditions[index].overcast;
            sky.cloudGloominess = conditions[index].gloominess;
        }

        
    }
    */
}
