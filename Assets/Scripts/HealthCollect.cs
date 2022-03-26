using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollect : MonoBehaviour {

    //heart object and collect sound
    [SerializeField]
    private Transform heart;
    [SerializeField]
    private Sound collectSound;
    private Transform soundParent;

    //player
    private Player player;


    //method called once at start
    void Start() {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        soundParent = GameObject.Find("Sounds").transform;
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
        
        //if the collision was with the player, give the player health, destroy this item and play collection sound
        if(other.gameObject.name == "Player") {
            player.GainHealth();
            Destroy(this.gameObject);
            Instantiate(collectSound, soundParent);
        }

    }
}
