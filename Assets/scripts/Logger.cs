using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class Logger : MonoBehaviour
{

    private StreamWriter sw;
    string pathroot = "SCD_STIMGEN_LOG";
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
        
        int i = 0;
        string newpath = pathroot + "_" + i.ToString("D2") + ext;
        while (File.Exists(newpath) & i < 100)
        {
            i++;
            newpath = pathroot + "_" + i.ToString("D2") + ext;

        }

        controller = GameObject.Find("ExperimentController").GetComponent<SCDSExperimentController>();
        
        sw = new StreamWriter(controller.folderPath + newpath); //append to logfile if it already exists  
        
    }

    public void LogEvent(string lineID, string logstring)
    {
        logstring = lineID + "\t" + logstring;

        //Debug.Log(logstring);
        sw.WriteLine(logstring);
        sw.Flush();
    }
}
