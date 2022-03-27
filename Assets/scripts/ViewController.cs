using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewController : MonoBehaviour {

    //The ViewController class provides a list of possible viewpoints from which scenes/views will be rendered
    //It also provides methods that check the visibility/occlusion of elements in the scene from different viewpoints

    //public List<Viewpoint> viewpoints = new List<Viewpoint>();
    //public float viewpointRadius=212.0f;
    public EnvironmentSpecifications env;
    public float viewpointElevation = 23.006f;
    public float viewpointRange = Mathf.PI / 3.0f; //default to 150 degrees
    public int nViewpoints = 3;
    public Viewpoint[,] TestViewpoints; 
    public List<float> anglesOfRotation;
    public GameObject teleportOrigin;
    public int currentview = 0;
    public bool clockwise = true;
    //[Range(0, 180)]
    //public int observationArc;
	// Use this for initialization
	void Start () {
        nViewpoints = anglesOfRotation.Count;
        TestViewpoints = new Viewpoint[nViewpoints, 2];        
        nViewpoints = anglesOfRotation.Count;
        currentview = Mathf.FloorToInt((nViewpoints - 1) / 2.0f); //centres currentview at the start
        teleportOrigin = GameObject.Find("TeleportOriginDesktop");
        //viewpoints.Clear();
        
        for (int i = 0; i < nViewpoints; i++) //this loop will calculate view angles and add viewpoints into multi-d array where each viewpoint can be stored (+ and -). it can be referenced as follows. 
            //TestViewpoints is a list of Viewpoint. Viewpoint datatype (defined below) contains a vector 2 denoting x and y coordinate. TestViewpoints[0,0] is vp1 TestViewpoints[0,1] is the mirror of this. TestViewpoints[1,0] is the first selected vp, TestViewpoints[1,1] is the mirror of this. etc for length nViewpoints 
        {          

            //calculate plus and minus angle
            for (int j = 0; j <= 1; j++)
            {
                if (j == 0) //calculate clockwise rotation and store in array
                {
                    Viewpoint v = new Viewpoint();
                    float thisAngleRads = Mathf.Deg2Rad * anglesOfRotation[i];
                    v.location.y  = -1*(Mathf.Cos(thisAngleRads) * (env.environmentRadius + 1f));//TRIG toi work out x and y. 3f is the distance from the array. this should be changed for different array sizes
                    v.location.x = Mathf.Sin(thisAngleRads) * (env.environmentRadius + 1f);
                    TestViewpoints[i,j] = v;
                    //Debug.Log(i +""+ j + " x= " + v.location.x + " y= " + " " + v.location.y + "");

                }
                else if (j == 1) //calculate anticlockwise rotation and store in array
                {
                    Viewpoint v = new Viewpoint();
                    v.location.y = TestViewpoints[i, 0].location.y;// second array item is reflection of first
                    v.location.x = TestViewpoints[i, 0].location.x*-1;
                    TestViewpoints[i, j] = v;
                    //Debug.Log(i + "" + j + " x= " + v.location.x + " y= " + " " + v.location.y + "");
                }
                                
            }
        }
     //debug
        for (int i  = 0; i <= nViewpoints-1; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                //Debug.Log(TestViewpoints.GetLength(0) + " " + TestViewpoints.GetLength(1));
                //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                //sphere.transform.position = new Vector3(TestViewpoints[i, j].location.x, 1.5F, TestViewpoints[i, j].location.y);
            }
        }
        currentview = 0;
        MoveView();
        
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            currentview++;
            if (currentview == nViewpoints)
            {
                currentview = 0;
            }
            MoveView();
        }		
	}

    public void MoveView()
    {
        //Debug.Log(currentview);
        //Debug.Log(TestViewpoints[currentview, 0]);
        //Debug.Log(TestViewpoints[currentview, 1]);
        //the +1.5f below denotes the viewpoint height above the floor
        float direction = Random.value;        
        if (!clockwise) //anticlockwise
        {
            teleportOrigin.transform.position = new Vector3(TestViewpoints[currentview, 0].location.x, env.viewHeight, TestViewpoints[currentview, 0].location.y);
            teleportOrigin.transform.LookAt(new Vector3(TestViewpoints[currentview, 0].lookat.x, env.arenafloor.transform.position.y+0.8f , TestViewpoints[currentview, 0].lookat.y)); // these define th height and position of viewpoints. 
        }
        else //clockwise
        {
            teleportOrigin.transform.position = new Vector3(TestViewpoints[currentview, 1].location.x, env.viewHeight, TestViewpoints[currentview, 1].location.y);
            teleportOrigin.transform.LookAt(new Vector3(TestViewpoints[currentview, 1].lookat.x, env.arenafloor.transform.position.y+0.8f, TestViewpoints[currentview, 1].lookat.y));
        }
    }
}

[System.Serializable]
public class Viewpoint{
    public Vector2 location;
    public Vector2 lookat = new Vector2(0, 0);
}
