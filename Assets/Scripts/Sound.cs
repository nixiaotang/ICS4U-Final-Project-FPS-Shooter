using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sound : MonoBehaviour
{

    //audio
    private AudioSource sound;

    void Start() {
        
        //get audio and play sound
        sound = GetComponent<AudioSource>();
        sound.Play();
    }

    // Update is called once per frame
    void Update() {
        
        //if sound is done playing, destroy this sound object
        if(!sound.isPlaying) Destroy(this.gameObject);
        
    }
}
