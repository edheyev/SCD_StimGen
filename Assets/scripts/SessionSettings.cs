using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SessionSettings : MonoBehaviour
{

    public string participantid;
    public string experimentcode;
    public string seedstring;
    public uint experimentseed;
    GameObject pptidtext;
    GameObject ectext;
    GameObject tvtext;
    GameObject seedtext;
    public string apptitle = "SCD_Table";
    public string version = "2.0";

    // Use this for initialization
    void Start()
    {
        DontDestroyOnLoad(this);
        pptidtext = GameObject.Find("PptIDText");
        ectext = GameObject.Find("ExperimentCodeText");
        tvtext = GameObject.Find("TitleVersionText");
        seedtext = GameObject.Find("RandomSeedText");
        tvtext.GetComponent<Text>().text = apptitle + " " + version;
    }

    public void Complete()
    {
        participantid = pptidtext.GetComponent<Text>().text;
        experimentcode = ectext.GetComponent<Text>().text;
        seedstring = seedtext.GetComponent<Text>().text;
        uint.TryParse(seedstring, out experimentseed);
    }
}
