using UnityEngine;
using System.Collections;
using System.IO;

public class Logger : MonoBehaviour
{

    private StreamWriter sw;
    string pathroot = "SCD_Log";
    string ext = ".txt";
    public SCDSExperimentController controller;
    SessionSettings sessionsettings;

    // Use this for initialization
    void Start()
    {
        
        sessionsettings = null;

        try
        {
            sessionsettings = GameObject.Find("SessionSettings").GetComponent<SessionSettings>();
        }
        catch
        {

        }

        if (sessionsettings != null)
        {
            if (sessionsettings.experimentcode.Length > 0 && sessionsettings.participantid.Length > 0)
            {
                pathroot = sessionsettings.experimentcode + "_" + sessionsettings.participantid + "_SCD_ViewChange_Table_Log";
            }
        }
        

        int i = 0;
        string newpath = pathroot + "_" + i.ToString("D2") + ext;
        while (File.Exists(newpath) & i < 100)
        {
            i++;
            newpath = pathroot + "_" + i.ToString("D2") + ext;

        }

        controller = GameObject.Find("ExperimentController").GetComponent<SCDSExperimentController>();
        if (controller.logswitch == SCDSExperimentController.SCDLoggerSwitch.SCDLS_LOG)
        {
            sw = new StreamWriter(newpath); //append to logfile if it already exists  
        }
    }

    public void LogEvent(string lineID, string logstring)
    {
        logstring = lineID + "\t" + logstring;

        //Debug.Log(logstring);
        sw.WriteLine(logstring);
        sw.Flush();
    }
}
