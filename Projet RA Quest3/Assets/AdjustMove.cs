using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.Interaction.Toolkit;
using Meta.XR.MRUtilityKit.SceneDecorator;
using System.Linq;
using System;
using Meta.XR.BuildingBlocks;
public class AdjustMove : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject leftController;
    public GameObject rightController;

    Vector3 leftControllerLast = Vector3.zero;
    Vector3 rightControllerLast = Vector3.zero;

    bool moving = false;
    bool rotating = false;
    bool clockWise = true;
    bool cClockWise = false;
    public float threshhold = 0.0001f;
    public float sensitivity = 0.1f;
    public void Start()
    {

    }
    
    public void OnOne()
    {
        moving = !moving;
    }
    public void OnTwo()
    {
        if (rotating && clockWise)
        {
            clockWise = false;
            cClockWise = true;
        }
        else if (rotating && cClockWise)
        {
            rotating = !rotating;
        }
        else if (!rotating)
        {
            clockWise = true ;
            cClockWise = false;
            rotating = true ;
        }
    }
    public void Update()
    {
        Vector3 leftControllerDelta = leftController.transform.position - leftControllerLast;
        Vector3 rightControllerDelta = rightController.transform.position - rightControllerLast;

        Debug.LogWarning(rightControllerDelta);
        if (moving)
        {
            if (rightControllerDelta.sqrMagnitude > threshhold)
            {
                this.transform.position += rightControllerDelta;

            }
        }
        if (rotating)
        { 
            if(leftControllerDelta.sqrMagnitude > threshhold && clockWise)
            {
                this.transform.rotation *= Quaternion.Euler(0, 0.05f, 0);
            }
            if(leftControllerDelta.sqrMagnitude > threshhold && cClockWise)
            {
                this.transform.rotation *= Quaternion.Euler(0, -0.05f, 0);
            }
        }
        leftControllerLast = leftController.transform.position; 
        rightControllerLast = rightController.transform.position;   
    }

}
