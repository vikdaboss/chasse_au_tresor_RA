using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.Interaction.Toolkit;
using Meta.XR.MRUtilityKit.SceneDecorator;
using System.Linq;
public class AdjustMove : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject leftController;
    bool bound = false;
    bool parented = false;
    public Vector3 worldAnchorPositionOffset = Vector3.zero;
    public Vector3 worldAnchorRotationOffset = Vector3.zero;
    public int ind = 0;
    void Start()
    {
    }
    /*
    public void OnBind()
    {
        bound = !bound;
    }*/
    public void Update()
    {/*
        if (bound)
        { 
            this.transform.position = leftController.transform.position;
            this.transform.rotation = leftController.transform.rotation;
        }*/
        if (!parented) 
        {
            OVRSpatialAnchor[] anchors = FindObjectsOfType<OVRSpatialAnchor>();
            if (anchors.Length > 0) {
                GameObject worldAnchor = anchors[ind].gameObject.transform.Find("Anchor Placement Transform").gameObject;
                this.transform.parent = worldAnchor.transform;
                this.transform.localPosition = worldAnchorPositionOffset;
                this.transform.localRotation = Quaternion.Euler(worldAnchorRotationOffset);
            }
        }

    }

}
