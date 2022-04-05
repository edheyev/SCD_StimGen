using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SCDSExperimentController : MonoBehaviour {


    SpatialViewGenerator svg;
    SpatialArrayGenerator sag;
    PrefabArrayGenerator pag;// this is the prefab array generator and is used for moultiple things including adding prefab elements to arrays and compiling into lists. also spawning prfabs. 
    ViewController viewController;
    SpatialDistortionManager distortionmanager; //used to control the amount of spatial distortion applied to the spatial foil landscapes
    Logger logger; // used to log experiment details. Info is stored in the 'SCDSControllerRecord' class (here - see below) and sent to logger to write to txt file.
     SCDControllerRecord record; // this is the record that will be updated each trial and sent to logger to be written.

    [Header("Batch customization")]
    public int totalTrialsToGen = 10;
    public List<float> spatialDistortions;
    public List<float> viewPoints;

    public int setSize;

    private int viewCount = 0;
    private TabletopElement targetElement;
    private GameObject targetObject;
    private Vector3 randomDirection;
    private bool distorted = false;
    private bool screenshotTaken = false;

    private Vector3 targetOrgPos;
    private TabletopElement foilElement;
    Camera cam;

    private enum SCDTutorialSwitch { SCDTS_TUTORIAL, SCDTS_TEST }
    private enum SCDTrialState { SCDTS_WAITING, SCDTS_PREPARE, SCDTS_STIMULUS1, SCDTS_STIMULUS2} //different phases of a trial
    private enum SCDExpState { SCDES_INIT, SCDES_PAUSE, SCDES_TRIALS, SCDES_ENDING } //different phases of overall experiment

    private SCDExpState expstate=SCDExpState.SCDES_INIT; //current state of experiment
    private SCDTrialState trialstate=SCDTrialState.SCDTS_WAITING; //current state of trial
    
    bool newstimulusrendered = false; //this flag is used to test whether a landscape has finished rendering (it can take several frames to update)

    private float numTrials = 0;
    private bool viewsRendered = false;
    private string locString;
    private string date;

    [Header("Current Trial")]
    public int trialCount = 0;

    [Header("Auto-generated batch folder")]

    public string folderPath;

    float maxDistInMeters;
    float minDistInMeters;
    private int numLevels;
    SessionSettings sessionsettings;
    int viewAngleIndex; //this is unique to the VC experiemnt and links staircases to viewpoints in spatial array generator
    int fadeframes = 0;
    bool newtrialstate; //flag used to avoid phase code running more than once at the beginning of a new phase
    private bool newsession = true;// used at experiment start once to log initial details.

    //trial progression
    private int sdId = 0;
    private int vpId = 0;
    private bool progress = false;
    private bool end = false;

    // Use this for initialization
    private void Awake()
    {
        Screen.SetResolution(1920, 1080, true);

        date = System.DateTime.Now.ToShortDateString();
        date = date.Replace("/", "-");
        date = date.Replace(" ", "_");
        date = date.Replace(":", "-");
        string time = System.DateTime.Now.ToShortTimeString();
        time = time.Replace("/", "-");
        time = time.Replace(" ", "_");
        time = time.Replace(":", "-");

        folderPath = "Batch_" + date + "_" + time + "/";

        if (!System.IO.Directory.Exists(folderPath))
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }
        else
        {
            System.IO.Directory.CreateDirectory(folderPath);
        }

    }
    void Start () {   
        //identify spatial view generator and staircase controllers
        svg = GameObject.Find("TableViewGenerator").GetComponent<TableViewGenerator>(); //
        pag = GameObject.Find("PrefabArrayGenerator").GetComponent<PrefabArrayGenerator>();
        viewController = GameObject.Find("ViewController").GetComponent<ViewController>();
        distortionmanager = gameObject.GetComponent<SpatialDistortionManager>();
        cam = GameObject.Find("DesktopCamera").GetComponent<Camera>(); ;
        logger = GameObject.Find("Logger").GetComponent<Logger>();
        record = new SCDControllerRecord();

        viewController.anglesOfRotation = viewPoints;

        svg.nElements = setSize; //set the (fixed) number of hills to use for precision trials       
    }

	
	// Update is called once per frame
	void Update () {

        if (newsession)
        {            
            //log experiment details at the start of the experiment.                                 
            logger.LogEvent("DATE", System.DateTime.Now.ToShortDateString() + "\t"+ System.DateTime.Now.ToShortTimeString() );
            logger.LogEvent("BatchInfo", "totalTrials" + "\t" + totalTrialsToGen + "\t" + "viewpoints" + "\t" + FloatListToText(viewController.anglesOfRotation) + "\t" + "distortions" + "\t" + FloatListToText(spatialDistortions) + "\n" ); 
            logger.LogEvent("DATA_NAMES", "screenshotFilename" + "\t" + "trialNum" + "\t" + "angleViewRotation" + "\t" + "rotationDirection"+ "\t" + "spatialDistortion" + "\t" + "numElements" + "\t" + "seed" + "\t"+ "cameraLoc" + "\t" + "cameraRot" + "\t" + "objectLocs");

            newsession = false;            
        }

		//switch trial and experiment states as appropriate
        switch (expstate)
        {
            case SCDExpState.SCDES_INIT:
                {
                   
                    expstate = SCDExpState.SCDES_PAUSE;
                    break;
                }
            case SCDExpState.SCDES_PAUSE:
                {                    
                    expstate = SCDExpState.SCDES_TRIALS;                        
                    trialstate = SCDTrialState.SCDTS_PREPARE;                    
                    break;
                }
            case SCDExpState.SCDES_TRIALS:
                {
                    //running trials
                    RunTrials();
                    break;
                }
            case SCDExpState.SCDES_ENDING:
                {
                    //tidying up and finishing
                    Application.Quit();
                    break;
                }
        }
	}

    void RunTrials()
    {
         
        
        switch(trialstate)
        {
            case SCDTrialState.SCDTS_PREPARE:
                {
                    
                    if (!viewsRendered)
                    {
                        DestroyItemsWithTag("clone");

                        sdId = 0;
                        vpId = 0;
                        svg.spatialDistortionLevel = 1;
                        svg.AppendView(SpatialViewType.SVT_SAMPLE);

                        var randInt = Random.Range(1, pag.arrayspec.Count) -1;
                        targetElement = pag.arrayspec[randInt];
                        string objectName = targetElement.profile.ToString();

                        svg.RenderView(0, vpId);
                        targetObject = GameObject.Find(objectName.Replace("TABLE_ELEMENT_", "").ToLower() + "(Clone)");
                       
                        float randRads = Random.Range(0, (2 * Mathf.PI));
                        randomDirection = new Vector3(Mathf.Sin(randRads),0, Mathf.Cos(randRads));                        
                        targetOrgPos = new Vector3(targetObject.transform.position.x, targetObject.transform.position.y, targetObject.transform.position.z); // target object is sometimes randomly destroyed... i cant figure out why. but lines 190-194 seem to compensate for it
                        viewsRendered = true;
                        
                    }

                    if (screenshotTaken)
                    {
                        screenshotTaken = false;
                        if (targetObject == null)
                        {
                            string objectName = targetElement.profile.ToString();
                            targetObject = GameObject.Find(objectName.Replace("TABLE_ELEMENT_", "").ToLower() + "(Clone)");
                        }
                        viewCount++;

                        viewController.currentview = vpId;
                        viewController.MoveView();


                        if (!distorted)
                        {                            
                            targetObject.gameObject.transform.position = targetOrgPos +  (randomDirection * spatialDistortions[sdId]);
                            distorted = true;
                        }

                        vpId++;                 
                        

                        if (vpId > viewController.anglesOfRotation.Count-1)
                        {
                            viewController.clockwise = !viewController.clockwise;
                            vpId = 0;

                            if(viewCount >= (viewController.anglesOfRotation.Count * 2) -1 )
                            {
                                viewCount = 0;
                                sdId++;
                                distorted = false;

                                if (sdId >= spatialDistortions.Count)
                                {
                                    sdId = 0;
                                    trialCount++;
                                    svg.randSeed++;
                                    DestroyItemsWithTag("clone");
                                    svg.ClearViews();
                                    viewsRendered = false;                 
                                }
                            }
                            
                            if(trialCount >= totalTrialsToGen)
                            {
                                trialstate = SCDTrialState.SCDTS_WAITING;
                                expstate = SCDExpState.SCDES_ENDING;
                            }
                        }
                    }
                    
                    string ssFileName = "_trial_" + trialCount + "_vp_" + viewController.anglesOfRotation[vpId] + "_sd_" + spatialDistortions[sdId]+"_date_"+ date + ".png";

                    ScreenCapture.CaptureScreenshot("./"+folderPath+ssFileName);

                    record.screenshotFilename = ssFileName;
                    record.trialNum = trialCount;
                    record.angleviewrotation = viewController.anglesOfRotation[vpId];
                    record.rotDirection = viewController.clockwise ? "clockwise" : "counter-clockwise";
                    record.spatialdistortion = spatialDistortions[sdId];
                    record.numelements = 4;
                    record.seed = (int)svg.randSeed;
                    record.cameraloc = "camPos:" + "_x:" + cam.transform.position.x.ToString() + "_y:" + cam.transform.position.y.ToString() + "_z:" + cam.transform.position.z.ToString();
                    record.camerarot = "camRot:" + "_x:" + cam.transform.rotation.x.ToString() + "_y:" + cam.transform.rotation.y.ToString() + "_z:" + cam.transform.rotation.z.ToString();

                    record.objectlocs = GetObjectLocs();
                    
                                    
                    logger.LogEvent("StimGen", record.LogString());
                    screenshotTaken = true;
                    
                    break;
                }
            case SCDTrialState.SCDTS_WAITING:
                {
                    Debug.Log("waiting");                   
                    break;
                }


        }
    }

    public void DestroyItemsWithTag(string tag)
    {
        GameObject[] destroyItems;
        destroyItems = GameObject.FindGameObjectsWithTag(tag);
        for (int i = 0; i < destroyItems.Length; i++)
        {
            Destroy(destroyItems[i].gameObject);
        }
    }

    private string IntListToText(List<int> list)
    {
        string result = "";
        foreach (var listMember in list)
        {
            result += listMember.ToString() + "\t";
        }
        return result;
    }

    private string FloatListToText(List<float> list)
    {
        string result = "";
        foreach (var listMember in list)
        {
            result += listMember.ToString() + "\t";
        }
        return result;
    }

    private string GetObjectLocs()
    {
        string result = "";
        GameObject[] objList =  GameObject.FindGameObjectsWithTag("clone");
        for (int i = 0; i < objList.Length; i++)
        {
            result += "Ob" + i + "_name: " + objList[i].name + "_x: " + objList[i].transform.position.x.ToString() + "_z: " + objList[i].transform.position.z.ToString();
        }
        return result;
    }
}

public class SCDControllerRecord
{
   
    public string screenshotFilename; //when response boxes become active
    public int trialNum; 
    public float angleviewrotation;
    public string rotDirection;
    public float spatialdistortion; //radial distance of spatial distortion
    public int numelements; //number of elements presented in array
    public int seed; //record random seed
    public string objectlocs;
    public string cameraloc;
    public string camerarot;



    public string LogString()
    {
        return screenshotFilename + "\t" + trialNum.ToString() + "\t" + angleviewrotation.ToString() + "\t" + rotDirection + "\t" + spatialdistortion.ToString() + "\t"  + numelements.ToString() + "\t" + seed.ToString() + "\t" +cameraloc + "\t" + camerarot + "\t" + objectlocs.ToString() + "\n";
    }
}

public class ExperimentSettings : MonoBehaviour
{
    public List<int> viewpoints;
    public List<float> spatialDistortions;
    public int setSize;

    public ExperimentSettings(List<int> _viewpoints, List<float> _spatialDistortions, int _setSize)
    {
        viewpoints = _viewpoints;
        spatialDistortions = _spatialDistortions;
        setSize = _setSize;
    }
}