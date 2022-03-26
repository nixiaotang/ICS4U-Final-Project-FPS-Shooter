using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookX : MonoBehaviour
{

    //player object
    [SerializeField]
    private Player player;

    // Update is called once per frame
    void Update() {

        //give mouse position, calculate the rotation of camera (changes where player looks)
        float mouseX = Input.GetAxis("Mouse X");
        Vector3 newRotation = transform.localEulerAngles;
        newRotation.y += mouseX * player.mouseSensitivity;

        //apply rotation
        transform.localEulerAngles = newRotation;

    }
}
