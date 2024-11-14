using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDetect : MonoBehaviour
{
    // Start is called before the first frame update
    bool foundMesh = false;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!foundMesh)
        {
            GameObject scan = GameObject.Find("GLOBAL_MESH_EffectMesh");
            if (scan != null)
            {
                foundMesh = true;
                OBJExporter.Export(scan);
            }
        }

    }
}
