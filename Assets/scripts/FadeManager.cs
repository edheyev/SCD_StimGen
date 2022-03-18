using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeManager : MonoBehaviour {

    //variables for fader
    public Texture2D fadeOutTexture;    //texture that will overlay the screen
    public float fadeSpeed = 0.8f;      // the fading speed

    private int drawDepth = -1000;      //texture order in 'draw heirarchy'  - low number = render on top
    private float alpha = 1.0f;         // texture alpha value 
    private int fadeDir = -1;           // the direction to fade (-1 fade in  1 fade out)


    private void OnGUI()
    {
        // fade out/in the alpha value using a direction, speed and time to convert to seconds. 
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        // force the number between 0 and 1 for alpha level
        alpha = Mathf.Clamp01(alpha);

        //set colour of our texture. colour remains the same - alpha is replaced by alpha
        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = drawDepth; // make sure black texture renders on top
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);  //draw fadouttexture on entire screen
    }

    //set fade direction to parameter -1 +1

    public void BeginFade(int direction, float speed)
    {
        fadeDir = direction;
        //return (speed); //return fadespeed variable so its easy to time with stimuli
    }

    //automate fading 

    void OnStimulusWasLoaded()
    {
        //alpha = 1    // use this if the apha is not set to 1 by default
        BeginFade(-1, 0.5f); //call the fade in function
    }

    
}
