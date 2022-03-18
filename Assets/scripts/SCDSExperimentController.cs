using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SCDSExperimentController : MonoBehaviour {

    // Spatial Change Detection Staircase Experiment Controller (SCDSExperimentController)

    // This high level script controls the sequence and timing of trials and within-trial phases in a spatial change detection staircase experiment.
    // The spatial stimuli themselves are constructed and rendered using SpatialViewGenerator and associated classes.
    // The adaptive staircase is handled by a StaircaseController.

    SpatialViewGenerator svg;
    SpatialArrayGenerator sag;
    public StaircaseController sc1; //start high
    public StaircaseController sc2; //start low   
    public StaircaseController scTutorial; //sc for tutorial
    private StaircaseController controller; //the current controller.
    public PrefabArrayGenerator pag;// this is the prefab array generator and is used for moultiple things including adding prefab elements to arrays and compiling into lists. also spawning prfabs. 

    public TabletopElement targetElement;
    public TabletopElement foilElement;
    Camera cam;

    public enum SCDTutorialSwitch { SCDTS_TUTORIAL, SCDTS_TEST }
    public enum SCDLoggerSwitch { SCDLS_LOG, SCDLS_DO_NOT_LOG } //choose to make record of experiment - default is ON
    public enum SCDStaircaseNum { SCDSN_ONE, SCDSN_TWO } //how many scs are we using
    public enum SCDTrialType { SCDTT_CAPACITY, SCDTT_PRECISION } //capacity trials - vary number of features (hills) based on participant responses, precision trials - vary degree of spatial distortion
    public enum SCDTrialState { SCDTS_WAITING, SCDTS_PREPARE, SCDTS_STIMULUS1, SCDTS_INTERSTIMULI_BLANK, SCDTS_STIMULUS2, SCDT_POSTSTIMULI_BLANK, SCDTS_RESPONSE, SCDTS_INTERTRIAL_BLANK} //different phases of a trial
    public enum SCDExpState { SCDES_INIT, SCDES_PAUSE, SCDES_TRIALS, SCDES_ENDING } //different phases of overall experiment
    public enum SCDTrialParity { SCDTP_SAME, SCDTP_DIFFERENT} //same trial (target landscape == sample landscape) or different trial (spatial foil != sample landscape)
    
    public SCDExpState expstate=SCDExpState.SCDES_INIT; //current state of experiment
    public SCDTrialState trialstate=SCDTrialState.SCDTS_WAITING; //current state of trial
    public SCDTrialParity currenttrialparity; //current parity (same-different trial)
    public SCDTrialType trialtype; //trial type to use for the current experiment
    public SCDLoggerSwitch logswitch;
    public SCDStaircaseNum numstairs;
    public string currentstair;
    public SCDTutorialSwitch tutorialSwitch;
    
    public float inittime=0.0f; //this time is used to display messages 
    public float stimulus1time = 1.0f; //time to display first stimulus (landscape)
    public float stimulus2time = 1.0f; //time to display second stimulus (landscape)
    public float interstimulustime = 0.5f; //time for blank screen between stimuli
    public float poststimulustime = 0.5f; //time for blank screen after second landscape and before response
    public float maxresponsetime = 5.0f; //maximum time allowed for a response before moving on to next trial
    public float intertrialtime = 0.5f; //time between trials
    public float fadetime=0.5f; //time for fade effect when moving between different phases of the trial - this is partly to avoid VR sickness resulting from rapid unanticipated 'jumps'
    bool newstimulusrendered = false; //this flag is used to test whether a landscape has finished rendering (it can take several frames to update)
    float spheretime; //used to control appearence/disppearence of spherical response environment (which surrounds the camera/headset location to give a featureless background during response phase)

    bool lasttrial;    
    public int maxTutTrials = 5;
    public float numTrials = 0;
    public float breaktrial = 30;

    public int participantResponse;

    SpatialDistortionManager distortionmanager; //used to control the amount of spatial distortion applied to the spatial foil landscapes

    public Logger logger; // used to log experiment details. Info is stored in the 'SCDSControllerRecord' class (here - see below) and sent to logger to write to txt file.
    public SCDControllerRecord record; // this is the record that will be updated each trial and sent to logger to be written.
    public float maxDistInMeters;
    public float minDistInMeters;
    public int numLevels;
    SessionSettings sessionsettings;
    public List<StaircaseController> validControllerList;
    public List<StaircaseController> originalControllerList;
    int viewAngleIndex; //this is unique to the VC experiemnt and links staircases to viewpoints in spatial array generator
    float nexttrialtime; //time to initiate next phase of the trial
    float exptime; //time to initiate next phase of the experiment
    int fadeframes = 0;
    bool newtrialstate; //flag used to avoid phase code running more than once at the beginning of a new phase
    private bool newsession = true;// used at experiment start once to log initial details.

    // Use this for initialization
    void Start () {

        
        lasttrial = false;
        //identify spatial view generator and staircase controllers
        svg = GameObject.Find("TableViewGenerator").GetComponent<TableViewGenerator>(); //
        pag = GameObject.Find("PrefabArrayGenerator").GetComponent<PrefabArrayGenerator>();
        distortionmanager = gameObject.GetComponent<SpatialDistortionManager>();

        cam = GameObject.Find("DesktopCamera").GetComponent<Camera>(); ;

        //configure staircase controller levels for capacity or precision trials as appropriate
        //these are currently hard coded which has the advantage of preserving them for the record, although it may be better to
        //read experimental parameters in from a file
        InitializeStaircase(); //note: you could in principle re-run this if necessary to change between trialtypes during the same session        
        controller = sc1;
        validControllerList = new List<StaircaseController>();

        switch (tutorialSwitch)
        {
            case SCDTutorialSwitch.SCDTS_TEST:
                {
                    for (int i = 0; i < svg.viewcontroller.anglesOfRotation.Count; i++)//make controllers for number of viewpoint angles
                    {
                        StaircaseController scnew = (StaircaseController)Instantiate(sc1);
                        scnew.transform.parent = this.transform;
                        validControllerList.Add(scnew);
                        scnew.name = "scBeginHigh_" + svg.viewcontroller.anglesOfRotation[i] + "_degrees";

                        if (numstairs == SCDStaircaseNum.SCDSN_TWO)
                        {
                            StaircaseController sc2new = (StaircaseController)Instantiate(sc2);
                            sc2new.transform.parent = this.transform;
                            validControllerList.Add(sc2new);
                            sc2new.name = "scBeginLow_" + svg.viewcontroller.anglesOfRotation[i] + "_degrees";
                        }
                    }
                    break;
                }
            case SCDTutorialSwitch.SCDTS_TUTORIAL:
                {
                    //tutorial stuff
                    //change sc
                    validControllerList.Clear();
                    StaircaseController scnew = (StaircaseController)Instantiate(scTutorial);
                    scnew.transform.parent = this.transform;
                    validControllerList.Add(scnew);
                    scnew.name = "scTutorialController";
                    //change view amount
                    
                   
                    //provide feedback?
                    break;
                }      
        }
        originalControllerList = new List<StaircaseController>(validControllerList); //this valid controller list will deplete as staircases complete
                      
        //identify logger and record
        logger = GameObject.Find("Logger").GetComponent<Logger>();
        record = new SCDControllerRecord();
              
        try // get session settings info
        {
            sessionsettings = GameObject.Find("SessionSettings").GetComponent<SessionSettings>();
            logger.LogEvent("VERSION", sessionsettings.apptitle + "\t" + sessionsettings.version + "\t" + trialtype + "\t" + "P_ID" + "\t" + sessionsettings.participantid);
            svg.randSeed = sessionsettings.experimentseed; //get specified random seed from splash as exp startng seed (each trial = seed ++)
        }
        catch
        {
            
        }
        
        if (trialtype==SCDTrialType.SCDTT_PRECISION) //trial type as defined in explorer
        {
            svg.nElements = 4; //set the (fixed) number of hills to use for precision trials
        }
        else
        {
            svg.spatialDistortionLevel = distortionmanager.AbsoluteDistortionFromStandard(1.0f); //set the (fixed) distortion level to use for capacity trials
        }
        exptime = Time.time + inittime;
    }

	
	// Update is called once per frame
	void Update () {

        if (newsession)
        {            
            if (logswitch == SCDLoggerSwitch.SCDLS_LOG) //make log
            {
                //log experiment details at the start of the experiment.                                 
                logger.LogEvent("DATE", System.DateTime.Now.ToShortDateString() + "\t"+ System.DateTime.Now.ToShortTimeString() );
                logger.LogEvent("STAIRCASE1_INFO", "NUp" + "\t" + sc1.nup + "\t" + "NDown" + "\t" + sc1.ndown + "\t" + "NReversals" + "\t" + sc1.nreversals + "\t" + "Levels" + "\t" + sc1.levels.Capacity +  "\t" + "BeginLow?" + "\t" + sc1.beginlow ); 
                if (numstairs == SCDStaircaseNum.SCDSN_TWO)
                {
                    logger.LogEvent("STAIRCASE2_INFO", "NUp" + "\t" + sc2.nup + "\t" + "NDown" + "\t" + sc2.ndown + "\t" + "NReversals" + "\t" + sc2.nreversals + "\t" + "Levels" + "\t" + sc2.levels.Capacity + "\t" + "BeginLow?" + "\t" + sc2.beginlow); 

                }
                logger.LogEvent("TRIAL_TIMINGS", "stimulus1timelimit" + "\t" + stimulus1time + "\t" + "stimulus2timelimit" + "\t" + stimulus2time + "\t" + "maxresponsetime" + "\t" + maxresponsetime + "\t" + "inter-stimulustime" + "\t" + interstimulustime + "\t" + "inter-trialtime" + "\t" + intertrialtime);
                logger.LogEvent("DATA_NAMES", "stim1time" + "\t" + "stim2time" + "\t" + "responseavailabletime" + "\t" + "responsegiventime" + "\t" + "response" + "\t" + "seed" + "\t" + "numelements" + "\t" + "spatialdistortion" + "\t" + "angleviewrotation" + "\t" + "currentstaircase" + "\t" + "sclevel" + "\t" + "sccurrentreversals"  + "\t" + "ptslocationstimulus1" + "\t" + "ptslocationstimulus2" + "\t" + "objectlocstim1" + "\t" + "objectlocstim2");
            }            
            newsession = false;            
        }

		//switch trial and experiment states as appropriate
        switch (expstate)
        {
            case SCDExpState.SCDES_INIT:
                {
                    //showing logo, messages etc.
                    if (Time.time>exptime)
                    {
                        expstate = SCDExpState.SCDES_PAUSE;
                        exptime = -1.0f; //negative: just to indicate that next phase is not timer initiated
                    }
                    break;
                }
            case SCDExpState.SCDES_PAUSE:
                {

                    if (Input.GetKeyDown(KeyCode.Insert))
                    {
                        Debug.Log("debug press");
                        expstate = SCDExpState.SCDES_TRIALS;                        
                    }


                    //waiting for experimenter to begin experiment
                    if (Input.GetKeyDown(KeyCode.Home))
                    {
                        expstate = SCDExpState.SCDES_TRIALS;                        
                        exptime = -1.0f; //negative: just to indicate that next phase is not timer initiated
                        trialstate = SCDTrialState.SCDTS_PREPARE;                        
                    }
                    break;
                }
            case SCDExpState.SCDES_TRIALS:
                {
                    //running trials
                    UpdateTrialPhase();
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

    void UpdateTrialPhase()
    {
        switch (trialstate)
        {
            case SCDTrialState.SCDTS_PREPARE:
                {

                    //objectHighlighter.ResetObjects(pag.distinctelements);
                    //create next stimulus pair etc.
                    //determine seed for new stimuli by incrementing svg seed
                    //Debug.Log("RANDSEED" + svg.randSeed);
                    svg.randSeed++;
                    numTrials ++; // tracks number of trials and incorperates breaks

                    //currenttrialparity = SCDTrialParity.SCDTP_DIFFERENT;

                    //are we going to generate a "same" or "different" trial?
                    
                    float rsamedifferent = Random.value;
                    //rsamedifferent = 0.7f;              
                    if (rsamedifferent > 0.5)
                    {
                        currenttrialparity = SCDTrialParity.SCDTP_DIFFERENT;                        
                    }
                    else
                    {
                        currenttrialparity = SCDTrialParity.SCDTP_SAME;
                    }
                    
                   
                    //then switch immediately to the next trial state
                    trialstate = SCDTrialState.SCDTS_STIMULUS1;
                    record.stim1time = Time.time;
                    nexttrialtime = Time.time + stimulus1time;
                    newstimulusrendered = false;
                    svg.ClearViews(); //this clears a list of views stored in the SVG


                   
                    // randomly choose staircase                     
                    //check which staircase and inform variable viewangleindex which will be sent to gen the array.
                    switch (tutorialSwitch)
                    {
                        case SCDTutorialSwitch.SCDTS_TEST:
                            {
                                //objectHighlighter.ResetObjects(pag.distinctelements);
                                if (validControllerList.Count > 0)
                                {
                                    Debug.Log("VALIDCOTROLLERLISTCOUNT" + validControllerList.Count);
                                    
                                    bool staircasesearching = true;
                                    while (staircasesearching) //look for valid staircase
                                    {
                                        int randomStaircaseSelection = Random.Range(0, validControllerList.Count);
                                        if ((validControllerList[randomStaircaseSelection].currentreversals >= validControllerList[randomStaircaseSelection].nreversals) || (validControllerList[randomStaircaseSelection].numTrialsPerformed >= validControllerList[randomStaircaseSelection].maxTrials))// && validControllerList[randomStaircaseSelection].ntrial <= maxTrials)//if valid.
                                        {
                                            validControllerList.RemoveAt(randomStaircaseSelection);
                                            if (validControllerList.Count == 0)//valid sc pool is depleted
                                            { //need to get new calid controller choice here?
                                                staircasesearching = false;
                                                Debug.Log("Experiment finished");
                                                trialstate = SCDTrialState.SCDTS_WAITING;
                                                expstate = SCDExpState.SCDES_ENDING;
                                                newtrialstate = true;
                                                lasttrial = true;
                                                return;
                                            }                              
                                        }
                                        else//remove chosen sc
                                        {
                                           //use chosen sc
                                            staircasesearching = false;
                                            controller = validControllerList[randomStaircaseSelection];
                                            currentstair = controller.name;                                           
                                            viewAngleIndex = originalControllerList.IndexOf(validControllerList[randomStaircaseSelection]); // view angle index is passed to the spatial array generator to inform viewpoint location.
                                            if (numstairs == SCDStaircaseNum.SCDSN_TWO)
                                            {
                                                viewAngleIndex = (int)Mathf.Floor(viewAngleIndex / 2); //if there are both up and down stairs the viewpoint index is half (floor) the normal one. 
                                            }
                                                record.currentstaircase = currentstair;
                                        }
                                    }
                                }
                                else //valid sc pool is depleted
                                {
                                    Debug.Log("Experiment finished");
                                    trialstate = SCDTrialState.SCDTS_WAITING;
                                    expstate = SCDExpState.SCDES_ENDING;
                                    newtrialstate = true;
                                    lasttrial = true;
                                    return;
                                }
                                break;

                            }
                        case SCDTutorialSwitch.SCDTS_TUTORIAL:
                            {
                                if (validControllerList[0].perceived_counter <= maxTutTrials)//participant must score n in a row to sucessfully end the tutorial phase. 
                                {
                                    //use chosen sc                                    
                                    controller = validControllerList[0];
                                    currentstair = controller.name;
                                    record.currentstaircase = currentstair;

                                    viewAngleIndex = 1; // view angle index is passed to the spatial array generator to inform viewpoint location.
                                    
                                }
                                else//remove chosen sc
                                {                                                           
                                    Debug.Log("Tutorial finished");
                                    trialstate = SCDTrialState.SCDTS_WAITING;
                                    expstate = SCDExpState.SCDES_ENDING;
                                    newtrialstate = true;
                                    lasttrial = true;
                                    SceneManager.LoadScene("Splash", LoadSceneMode.Single);
                                    return;                                    
                                }
                                break;
                            }
                    }
                                                                  

                     

                    //this next piece of code alters the changable element in the trial (number of objects or amount of spatial distortion) based on the staircase level.                     
                    if (trialtype == SCDTrialType.SCDTT_CAPACITY)
                    {
                        svg.nElements = (int)controller.levels[controller.currentlevel]; //the staircase levels are set to run in integer steps from 1, starting low. The lowest possible number of hills in the array should be three.
                        //warning: the staircase controller is typically set so that sc.levels[sc.currentlevel] is a 0-based positive integer i.e., the same as sc.currentlevel
                        //but the idea is that sc.levels can be used for to set different values - however to allow for capacity and precision trials in the same code we convert the integers to the values we need.
                        //in this case we just add the minimum number of mountains to the current staircase level.
                                                
                    }
                    else
                    {
                        //alter distortion values in svg
                        svg.spatialDistortionLevel = distortionmanager.AbsoluteDistortionFromStandard(controller.levels[controller.currentlevel]);
                    }
                    
                    //record array values 
                    record.seed = (int)svg.randSeed;
                    record.numelements = svg.nElements; //record num elements
                    record.spatialdistortion = svg.spatialDistortionLevel; //record distance distorted 
                    record.sclevel = controller.currentlevel; //record sc level
                    record.sccurrentreversals = controller.currentreversals; //record sc current reversals
                    record.angleviewrotation = svg.viewcontroller.viewpointRange/(svg.viewcontroller.nViewpoints - 1);

                    /*
                    //always different
                    svg.AppendView(SpatialViewType.SVT_SAMPLE);
                    svg.AppendView(SpatialViewType.SVT_SPATIAL); //spatial foil generated using current values of svg public variables 
                    //svg.AppendView(SpatialViewType.SVT_TARGET); //a different view of the same place
                      */                

                    //set up views for next trial
                    switch (currenttrialparity)
                    {
                        //consider swapping order randomly in this section - i.e., it should be possible for the 'sample' stimulus to come second                        
                        case SCDTrialParity.SCDTP_SAME:
                            svg.AppendView(SpatialViewType.SVT_SAMPLE);
                            svg.AppendView(SpatialViewType.SVT_TARGET); //a different view of the same place
                            break;
                        case SCDTrialParity.SCDTP_DIFFERENT:
                            svg.AppendView(SpatialViewType.SVT_SAMPLE);
                            svg.AppendView(SpatialViewType.SVT_SPATIAL); //spatial foil generated using current values of svg public variables 
                            //svg.AppendView(SpatialViewType.SVT_TARGET); //a different view of the same place
                            break;
                    }
                    
                    
                    //record.ptslocstim2 = svg.viewcontroller.viewpoints[svg.views[1].viewpoint].location;

                    newtrialstate = true;
                    
                    break;
                }
            case SCDTrialState.SCDTS_STIMULUS1:
                {
                    //check to see whether stimulus has already been spawned/built/rendered
                    //if not, render stimulus1
                    if (!newstimulusrendered)
                    {
                        //render stimulus before moving on                                             
                        pag.noise_seed = svg.views[0].noise_seed;                        
                        svg.RenderView(0);
                        record.ptslocstim1 = "("+svg.viewcontroller.TestViewpoints[0, 0].location.x.ToString() + ", " + svg.viewcontroller.TestViewpoints[0, 0].location.y.ToString() + ")";
                        newstimulusrendered = true; //it will actually take a couple of frames to update.
                    }

                    //set fader
                    
                    if (pag.state == PrefabArrayGenerator.TableState.TS_IDLE)
                    {
                    }
                    else
                    {
                    }
                    
                    //check timer, continue to present stimulus 1 until designated time
                    //then switch to the next trial state
                    if (Time.time>nexttrialtime)
                    {
                        trialstate = SCDTrialState.SCDTS_INTERSTIMULI_BLANK;                        
                        nexttrialtime = Time.time + interstimulustime;
                        newtrialstate = true;
                    }
                    else
                    {
                        newtrialstate = false;
                    }

                    record.objectlocstim1 = null;
                    //record stim 1 information
                   // Debug.Log(pag.arrayspec.Count);
                    for (int i = 0; i <= pag.arrayspec.Count-1; i++)
                    {
                        //Debug.Log(i);
                        //Debug.Log(pag.arrayspec[i].prefabtype);
                        record.objectlocstim1 += pag.arrayspec[i].prefabtype.ToString() + "\t" + pag.arrayspec[i].centre.ToString() + "\t";
                    }

                    break;                   
                }
            case SCDTrialState.SCDTS_INTERSTIMULI_BLANK:
                {
                    if (newtrialstate)
                    {
                    }

                    if (Time.time > nexttrialtime)
                    {
                        trialstate = SCDTrialState.SCDTS_STIMULUS2;
                        record.stim2time = Time.time;
                        nexttrialtime = Time.time + stimulus2time;
                        newstimulusrendered = false;
                        newtrialstate = true;
                    }
                    else
                    {
                        newtrialstate = false;
                    }
                    break;

                }
            case SCDTrialState.SCDTS_STIMULUS2:
                {
                    //check to see whether stimulus has already been spawned/built/rendered
                    //if not, render stimulus1
                    if (!newstimulusrendered)
                    {
                        //render stimulus before moving on
                        pag.noise_seed = svg.views[1].noise_seed;
                        //svg.views[1].viewpoint = viewAngleIndex;
                        svg.RenderView(1, viewAngleIndex);
                        record.ptslocstim2 = "(" + svg.viewcontroller.teleportOrigin.transform.position.x.ToString("F2") + ", " + svg.viewcontroller.teleportOrigin.transform.position.z.ToString("F2") + ")";
                        

                        newstimulusrendered = true; //it will actually take a couple of frames to update.
                    }

                    //set fader
                    
                    if (pag.state == PrefabArrayGenerator.TableState.TS_IDLE)
                    {
                    }
                    else
                    {
                    }
                    

                    //check timer, continue to present stimulus 1 until designated time
                    //then switch to the next trial state
                    if (Time.time > nexttrialtime)
                    {
                        trialstate = SCDTrialState.SCDT_POSTSTIMULI_BLANK;
                        spheretime = Time.time + fadetime;
                        nexttrialtime = Time.time + poststimulustime;
                        newtrialstate = true;
                    }
                    else
                    {
                        newtrialstate = false;
                    }

                    record.objectlocstim2 = null;
                    //record stim 2 information
                    for (int i = 0; i <= pag.arrayspec.Count - 1; i++)
                    {
                        record.objectlocstim2 += pag.arrayspec[i].prefabtype.ToString() + "\t" + pag.arrayspec[i].centre.ToString() + "\t";
                    }

                    break;
                }
            case SCDTrialState.SCDT_POSTSTIMULI_BLANK:
                {
                    if (newtrialstate)
                    {
//                        objectHighlighter.HighlightTwo(pag.arrayspec[pag.indextomove], pag.arrayspec); //highlight two objects
                        //objectHighlighter.HighlightTwoAndOcclude(pag.arrayspec[pag.indextomove], pag.arrayspec); //highlight two objectsand occlude others


                    }

                    if (Time.time > spheretime)
                    {

                    }

                    if (Time.time > nexttrialtime)
                    {
                        trialstate = SCDTrialState.SCDTS_RESPONSE;
                        nexttrialtime = Time.time + maxresponsetime;
                        newtrialstate = true;
                        //responseboxes.GetComponent<ResponseBoxGroup>().ResetResponseBoxGroup();
                        record.responseavailabletime = Time.time; // start of response period                  
                    }
                    else
                    {
                        newtrialstate = false;
                    }
                    break;
                }
            case SCDTrialState.SCDTS_RESPONSE:
                {
                    participantResponse = -1;

                    if (newtrialstate)
                    {
                        //objectHighlighter.HighlightTwo(pag.arrayspec[pag.indextomove], pag.arrayspec); //highlight two objects
                    }

                    RaycastHit hit;
                    Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                                       
                    if (true)
                    {
                        //Debug.Log(hit.transform.name);
                        if(currenttrialparity == SCDTrialParity.SCDTP_SAME)
                        {
                           // objectHighlighter.HighlightTarget(targetElement, objectHighlighter.color2);
                            if (Input.GetKeyDown("s"))
                                {
                                    //process response correct
                                    Debug.Log("CORRECT");
                                    participantResponse = 0;
                                }
                            else if (Input.GetKeyDown("d"))
                            {
                                Debug.Log("INCORRECT");
                                participantResponse = 1;
                            }
                        }
                        else if (currenttrialparity == SCDTrialParity.SCDTP_DIFFERENT)
                            {
                           // objectHighlighter.HighlightTarget(foilElement, objectHighlighter.color2);
                            if (Input.GetKeyDown("s"))
                            {
                                    //process incorrect response
                                    Debug.Log("INCORRECT");
                                    participantResponse = 1;
                            }
                            else if (Input.GetKeyDown("d"))
                            {
                                Debug.Log("CORRECT");
                                participantResponse = 0;
                            }
                        }
                        else
                        {
                          //  objectHighlighter.HighlightTarget(targetElement, objectHighlighter.color1);
                         //   objectHighlighter.HighlightTarget(foilElement, objectHighlighter.color1);
                        }
                    }              

                if (Time.time > nexttrialtime || (participantResponse >=0)) //or check for response
                    {
                    record.responsegiventime = Time.time; // response (or no response) made.
                //check whether staircase is complete here
                if (lasttrial) // this has to be changed for 2 scs - also sort of doing this in the trial prep bit?
                {
                    trialstate = SCDTrialState.SCDTS_WAITING;
                    expstate = SCDExpState.SCDES_ENDING;
                    newtrialstate = true;
                }
                else
                {
                    if (participantResponse == 0) //participant is correct
                    {
                        //movestaircase down                          
                        controller.ProcessResponse(true); //change staircase currentlevel                                                            
                        record.response = SCDControllerRecord.SCDCR_RESPONSE.SCDCR_RESPONSE_CORRECT; //record pts response                             
                    }
                    else if (participantResponse == 1) //participant incorrect
                    {
                        // move staircase up
                        controller.ProcessResponse(false);//change staircase currentlevel                
                        record.response = SCDControllerRecord.SCDCR_RESPONSE.SCDCR_RESPONSE_INCORRECT; //record pts response
                    }
                    else //participant did not answer
                    {
                        //record no response
                        record.response = SCDControllerRecord.SCDCR_RESPONSE.SCDCR_RESPONSE_NO_RESPONSE; //record pts response
                    }

                    

                           if (logswitch == SCDLoggerSwitch.SCDLS_LOG) //write record to log file
                            {
                                logger.LogEvent("SCDS", record.LogString());
                            }
                           //reset various bits
                           // objectHighlighter.ResetObjects(pag.distinctelements);
                            trialstate = SCDTrialState.SCDTS_INTERTRIAL_BLANK;
                            spheretime = Time.time + fadetime;
                            nexttrialtime = Time.time + intertrialtime;
                            newtrialstate = true;
                        }                        
                    }
                    else
                    {
                        newtrialstate = false;                        
                    }
                    break;
                }
            case SCDTrialState.SCDTS_INTERTRIAL_BLANK:
                {
                    if (newtrialstate)
                    {
                    }

                    if (Time.time > spheretime)
                    {
                    }

                    if (Time.time>nexttrialtime)
                    {
                        if (numTrials % breaktrial ==0f)
                        {
                            nexttrialtime = -1.0f; //signifying that the next change is not timer initiated.
                            newstimulusrendered = false;
                            newtrialstate = true;
                            expstate = SCDExpState.SCDES_PAUSE;
                            numTrials++;
                        }
                        else
                        {
                            trialstate = SCDTrialState.SCDTS_PREPARE;
                            nexttrialtime = -1.0f; //signifying that the next change is not timer initiated.
                            newstimulusrendered = false;
                            newtrialstate = true;
                        }                        
                    }
                    else
                    {
                        newtrialstate = false;
                    }
                    break;
                }
        }
    }
    void InitializeStaircase()
    {
        if (trialtype == SCDTrialType.SCDTT_CAPACITY)
        {
            //capacity levels start at 3 mountains and go up to 9. Generating more than 9 item stimuli is problematic and can lead to crashes
            sc1.levels.Clear();
            sc2.levels.Clear();

            for (int i = 7; i >=0; i--)
            {
                sc1.levels.Add(i + 3); //as level index increases, number of mountains is reduced, making the test easier(?)
                sc2.levels.Add(i + 3);
            }

            //the number of reversals determines the overall length of the experiment
            //sc1.nreversals = 11; //use 11 reversals
            //sc2.nreversals = 11;
            sc1.nup = 1; //use a 1-up, 3-down staircase - should home in at about 79% correct level
            sc2.nup = 1;
            //sc1.ndown = 3;
            //sc2.ndown = 3;

            sc2.beginlow = true;
            sc1.beginlow = false;
            

            //for each reversal except the last we need to determine how many steps will be jumped following a response
            //large jumps are sometimes used to establish coarse tuning at the beginning of the experiment
            //however for capacity trials we use a single step throughout

            sc1.stepsizes.Clear();
            sc2.stepsizes.Clear();
            for (int i = 0; i <= 7; i++)
            {
                sc1.stepsizes.Add(1);
                sc2.stepsizes.Add(1);
            }
        }
        else
        {
            //for precision trials, distortion levels are based on a 'standard distortion level' called 1, and will range from 1/2 to 2 times this level in log spaced steps (corresponding to -6 to 6dB)
            //the spatial distortion manager will convert these standard units into physical units (meters, degrees) used to generate spatial foils
                                   
            sc1.levels.Clear();
            sc2.levels.Clear();
            //int numlevels = 19;
            
            float minvalonscale = minDistInMeters / distortionmanager.standard_distortion_radius; // get correct multiplier for sc level based on min and max and average dstance
            
            float maxvalonscale = maxDistInMeters / distortionmanager.standard_distortion_radius;

            float[] q = LinSpace(Mathf.Log(minvalonscale), Mathf.Log(maxvalonscale), numLevels); // get linearly spaced numbers between 2 points. - use log because we want log spaced between 2 points.

            for (int i = 0; i < numLevels; i++)
            {
                //float q = Mathf.Pow(10, (float)((i - 9f) / 9f) * 0.45f); //as level index increases the size of spatial distortion increases, making the test easier

                //9f = (numlevels-1)/2  ... ths generates from 0.1 - 10 with 1 as central value. the * 0.45 second value defines the max and min values while keeping 1 in the middle of the array/
                q[i] = Mathf.Exp(q[i]); //exp to get log spaced

                sc1.levels.Add(q[i]);
                sc2.levels.Add(q[i]);
            }

            //the number of reversals determines the overall length of the experiment
            //sc1.nreversals = 11; //use 11 reversals
            sc1.nup = 1; //use a 1-up, 3-down staircase - should home in at about 79% correct level
            //sc1.ndown = 3;
            //sc2.nreversals = 11; //use 11 reversals
            sc2.nup = 1; //use a 1-up, 3-down staircase - should home in at about 79% correct level
            //sc2.ndown = 3;

            sc1.beginlow = false;
            sc2.beginlow = true;

            //for each reversal except the last we need to determine how many steps will be jumped following a response
            //large jumps are sometimes used to establish coarse tuning at the beginning of the experiment
            //however for precision trials we will use a large initial step size to begin with and then rapidly reduce to a single step

            sc1.stepsizes.Clear();
            sc2.stepsizes.Clear();
            Debug.Log(sc1.stepsizes.Count);
            Debug.Log(numLevels);
            for (int i = 0; i < sc1.nreversals-1; i++)
            {
                Debug.Log("hello");
                if (i < 2)
                {
                    sc1.stepsizes.Add(Mathf.FloorToInt(Mathf.Pow(2, 2 - i)));
                    sc2.stepsizes.Add(Mathf.FloorToInt(Mathf.Pow(2, 2 - i)));
                }
                else
                {
                    sc1.stepsizes.Add(1);
                    sc2.stepsizes.Add(1);
                    
                }
            }
        }
        sc1.Initialize();
        sc2.Initialize();      
    }


    private static float[] LinSpace(float x1, float x2, int n)
    {
        //x1 = min x2 = max, n = number points
        float step = (x2 - x1) / (n - 1); //step size
        float[] y = new float[n];         // array to hold output values
        for (int i = 0; i < n; i++)
        {
            y[i] = x1 + step * i;
        }
        return y;        
    }
}

public class SCDControllerRecord
{
    //public enum SCDCR_TRIALTYPE { SCDCR_REFERENCE_SAME, SCDCR_REFERENCE_DIFFERENT };
    public enum SCDCR_RESPONSE { SCDCR_RESPONSE_CORRECT, SCDCR_RESPONSE_INCORRECT, SCDCR_RESPONSE_NO_RESPONSE };
    //public enum SCDCR_CURRENTSTAIRCASE { SCDCR_STAIRCASE_ONE, SCDCR_STAIRCASE_TWO }
    //public enum SCDS_BIGGER { RSSCR_BIGGER_REFERENCE, RSSCR_BIGGER_COMPARE, RSSCR_BIGGER_NO_RESPONSE };

    //below- variables written to log line in order
    //pts response info
    public float stim1time;
    public float stim2time;
    public float responseavailabletime; //when response boxes become active
    public float responsegiventime; //when response is made
    //public SCDCR_TRIALTYPE trialparity;
    public SCDCR_RESPONSE response;
    //array and presentation info
    public int seed; //record random seed
    public int numelements; //number of elements presented in array
    public float spatialdistortion; //radial distance of spatial distortion
    public float angleviewrotation; // THIS IS CURRENTLY STATIC BASED ON INITIAL SETTINGS
    //staircase info
    public string currentstaircase;
    public int sclevel;    
    public int sccurrentreversals;
    public bool screversaltrial;

    public string ptslocstim1;
    public string ptslocstim2;

    public string objectlocstim1;
    public string objectlocstim2;

    public string LogString()
    {
        return stim1time + "\t" + stim2time + "\t" + responseavailabletime + "\t" + responsegiventime + "\t"  + response.ToString() + "\t" + seed.ToString() + "\t" + numelements.ToString() + "\t" + spatialdistortion.ToString() + "\t" + angleviewrotation.ToString() + "\t"+ currentstaircase.ToString() + "\t" + sclevel.ToString() + "\t" + sccurrentreversals.ToString() + "\t" +  ptslocstim1.ToString() + "\t" + ptslocstim2.ToString() + "\t" + objectlocstim1 + "\t" + objectlocstim2 + "\n";
    }
}