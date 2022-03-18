using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentSpecifications : MonoBehaviour {

    public enum ENVIRONMENT { E_MOUNTAIN, E_TABLETOP }
    public ENVIRONMENT environment;
    public GameObject boundarytemplate;   //place a cyliner primative with the top edge inline with where you want objects to spawn. transform.scale.y should be 1. change scale.x and z to fit the circular arena
    
    //public Vector2 centre;
    public float environmentRadius;
    public float maxAbsRadius;
    public float minAbsRadius;
    public float maxDeltaRadius;
    public float minDeltaRadius;
    public float maxAbsAngle;
    public float minAbsAngle;
    public float maxDeltaAngle;
    public float minDeltaAngle;
    public float viewHeight;
    public float maxrad;
   
    public Transform arenafloor;

    void Awake()
    {
        float maxrad = boundarytemplate.transform.localScale.x /2f;
        arenafloor = boundarytemplate.transform.Find("floor");
        //Vector3 floorRelativeToWorld = transform.TransformPoint(arenafloor.transform.position);
        //floorHeight = floorRelativeToWorld.y;


        switch (environment)
        {
            case ENVIRONMENT.E_MOUNTAIN:
                {
                    /*
                    environmentRadius;
                    maxAbsRadius;
                    minAbsRadius;
                    maxDeltaRadius;
                    minDeltaRadius;
                    maxAbsAngle;
                    minAbsAngle;
                    maxDeltaAngle;
                    minDeltaAngle;
                    */
                    break;
                    
                }
            case ENVIRONMENT.E_TABLETOP:
                {
                    environmentRadius = maxrad;
                    maxAbsRadius = maxrad - maxrad*0.1f;
                    minAbsRadius = maxrad/10f;
                    maxDeltaRadius = (maxrad/2f) + maxrad/3f;
                    minDeltaRadius = (maxrad/2f) - maxrad/3f;
                    //maxAbsAngle;
                    //minAbsAngle;
                    maxDeltaAngle  =0.7854f; 
                    minDeltaAngle = 0.3926f;
                    break;
                }
        }
    }
}
