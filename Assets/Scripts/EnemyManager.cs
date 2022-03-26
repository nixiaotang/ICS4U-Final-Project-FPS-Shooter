using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyManager : MonoBehaviour
{
    
    //if curerntly spawning enemy
    private bool spawnWaiting = false;

    //important objects
    [SerializeField]
    private GameObject enemyObj;
    [SerializeField]
    private Transform player;


    // Update is called once per frame
    void Update() {
        
        //if not spawning enemy, start spawning a new enemy
        if(!spawnWaiting) {
            spawnWaiting = true;

            StartCoroutine(EnemySpawnCoolDown());
        }
        
    }

    //method returns a spot on the map to generate an enemy
    private Vector3 genEnemyPosition() {
        
        //See Enemy.cs for more details

        if(Random.Range(0f, 1f) > 0.5) {

            Vector2 randCircle = Random.insideUnitCircle * 45;
            Vector3 randomPos = new Vector3(randCircle.x-14, 3, randCircle.y + 37);
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 45, 1)) {
                if((player.transform.position - hit.position).magnitude < 10) return genEnemyPosition();
                else return hit.position;
            }

        } else {

            Vector2 randCircle = Random.insideUnitCircle * 35;
            Vector3 randomPos = new Vector3(randCircle.x-67, 3, randCircle.y + 30);
            
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPos, out hit, 35, 1)) {
                if((player.transform.position - hit.position).magnitude < 10) return genEnemyPosition();
                else return hit.position;
            }
        }

        return Vector3.zero;
    }

    //method to spawn new enemy
    private void SpawnEnemy() {
        Instantiate(enemyObj, genEnemyPosition(), Quaternion.identity, this.gameObject.transform);

    }

    //cool down for enemy spawn time
    IEnumerator EnemySpawnCoolDown() {

        SpawnEnemy();

        yield return new WaitForSeconds(15f);

        spawnWaiting = false;

    }
}
