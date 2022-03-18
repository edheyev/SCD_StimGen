using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardButton : MonoBehaviour
{ 
    public int RecordKeyPress()
    {
        
        int outpress = 9;
        
            if (Input.GetKeyDown("s"))
            {
                outpress = 0;
               
            }
            else if (Input.GetKeyDown("d"))
            {
                outpress = 1;
               
            }
        
        return outpress;
    }
}
