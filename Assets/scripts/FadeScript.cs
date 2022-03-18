using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;



public class FadeScript : MonoBehaviour
{
    public Image black;

    /*
    //private IEnumerator Start()
    {
        black.canvasRenderer.SetAlpha(0.0f);
        yield return new WaitForSeconds(2.5f);
    }
    */
    public void FadeIn(float fadeSpeed)
    {
        black.CrossFadeAlpha(1.0f, fadeSpeed, false);    
        //yield return new WaitForSeconds(2.5f);
    }

    public void FadeOut(float fadeSpeed)
    {
        black.CrossFadeAlpha(0.0f, fadeSpeed, false);
    }
}