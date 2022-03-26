using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    private Transform cam;

    void Start() {
        
        //get camera object
        cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Transform>();
        
    }


    void LateUpdate() {

        //rotate object to face the camera
        transform.LookAt(transform.position + cam.forward);
        
        
    }
}
