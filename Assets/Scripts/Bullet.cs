using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {
    
    private float speed = 30f;                 //bullet speed
    private static Player player;              //player object
    private static GameObject hitMarker;       //effects during collision


    void Start() {
        transform.position += transform.forward * 2;    //starting position
    }


    //set collision effect object
    public void setHitMarker(GameObject obj) {
        hitMarker = obj;
    }


    void Update() {

        //get player object
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        //continue moving forward
        transform.position += transform.forward * speed * Time.deltaTime;
        
    }

    public void OnTriggerEnter(Collider other) {

        //if collides with player, communicate hit to player object
        //if collided with something else, play collision effects
        if(other.gameObject.name == "Player") player.Hit();
        else Instantiate(hitMarker, other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position), Quaternion.identity);
        
        //destroy bullet
        Destroy(this.gameObject);

    }
}
