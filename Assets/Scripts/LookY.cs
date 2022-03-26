using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookY : MonoBehaviour
{

    //player object
    [SerializeField]
    private Player player;

    // Update is called once per frame
    void Update() {

        //give mouse position, calculate the rotation of camera (changes where player looks)
        float mouseY = Input.GetAxis("Mouse Y");
        Vector3 newRotation = transform.localEulerAngles;
        newRotation.x -= mouseY * player.mouseSensitivity;

        //prevents going too far up or down
        if(newRotation.x > 200) newRotation.x = Mathf.Max(newRotation.x, 310);
        else newRotation.x = Mathf.Min(newRotation.x, 55);

        //apply rotation
        transform.localEulerAngles = newRotation;

    }
}
