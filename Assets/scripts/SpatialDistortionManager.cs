using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpatialDistortionManager : MonoBehaviour {
    // this spatial distortion manager converts staircase level into spatial distortion information
    public EnvironmentSpecifications env;       
    public float standard_distortion_radius = 60; //absolute distance that feature will be moved following spatial distortion

    private void Awake()
    {
        //standard_distortion_radius = env.environmentRadius * 0.49f;
        //standard_distortion_radius = 1.3f;

    }
    public float AbsoluteDistortionFromStandard(float distortion_level)
    {
        return standard_distortion_radius * distortion_level;
    }
}

