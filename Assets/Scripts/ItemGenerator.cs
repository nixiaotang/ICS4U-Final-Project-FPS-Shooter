using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ItemGenerator : MonoBehaviour
{
    
    //whether or not something is spawning
    private bool spawnWaiting = false;

    //items to spawn (handgun, rifle or heart) objects
    [SerializeField]
    private GameObject handGun, rifle, heart;

    //player object
    [SerializeField]
    private Transform player;


    // Update is called once per frame
    void Update() {

        //if not currently spawning something, start spawning something
        if(!spawnWaiting) {
            spawnWaiting = true;

            StartCoroutine(itemSpawnCoolDown());
        }
        
    }

    //method to get a position on the map to generate an item
    private Vector3 genItemPosition() {

        //For more details, see genEnemyPosition() in Enemy.cs

        if(Random.Range(0f, 1f) > 0.5) {

            Vector2 randCircle = Random.insideUnitCircle * 45;
            Vector3 randomPos = new Vector3(randCircle.x-14, 3, randCircle.y + 37);
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 45, 1)) {
                if((player.transform.position - hit.position).magnitude < 10) return genItemPosition();
                else return hit.position;
            }

        } else {

            Vector2 randCircle = Random.insideUnitCircle * 35;
            Vector3 randomPos = new Vector3(randCircle.x-67, 3, randCircle.y + 30);
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 35, 1)) {
                if((player.transform.position - hit.position).magnitude < 10) return genItemPosition();
                else return hit.position;
            }
        }

        return Vector3.zero;
    }

    //method to spawn a new item
    private void spawnItem() {

        Vector3 pos = genItemPosition();           //position to spawn the new item
        float randItem = Random.Range(0f, 1f);     //random to determine what to spawn

        //instantiates new item
        if(randItem > 0.70f) Instantiate(handGun, pos, Quaternion.identity, this.gameObject.transform);         //30% spawn a handgun
        else if(randItem > 0.4f) Instantiate(rifle, pos, Quaternion.identity, this.gameObject.transform);       //30% spawn a rifle
        else Instantiate(heart, pos, Quaternion.identity, this.gameObject.transform);                           //40% spawn a healthboost

    }

    //spawning cool down
    IEnumerator itemSpawnCoolDown() {
        
        //spawn new item, wait for 20 seconds, indicate stop spawning
        spawnItem();

        yield return new WaitForSeconds(20f);

        spawnWaiting = false;

    }
}
