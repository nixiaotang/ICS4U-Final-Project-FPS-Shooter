using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleCollect : MonoBehaviour {

    //gun and sound objects
    [SerializeField]
    private Transform rifle;
    [SerializeField]
    private Sound collectSound;
    private Transform soundParent;

    //player object
    private Player player;

    //method called at start
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    //method called at each frame
    void Update() {
        
        //slowly moves object up and down (floating effect)
        float y = 2f + Mathf.Sin(Time.frameCount/60f) * 0.1f;

        //slowly rotates the object around
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
        transform.localEulerAngles += new Vector3(0, 0.1f, 0);
        
    }

    //called when a collision occurs
    public void OnTriggerEnter(Collider other) {

        //if the collision was with the player, give the player the rifle, destroy this item and play collection sound
        if(other.gameObject.name == "Player") {
            player.CollectRifle();
            Destroy(this.gameObject);
        }

    }
}
