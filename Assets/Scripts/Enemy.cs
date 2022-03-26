using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour {

    //Magic numbers
    private int WANDER = 0, CHASE = 1, SHOOT = 2, FIND = 3, LOOKAROUND = 4;

    //damage (headshot, bodyshot, leg shot)
    private int []damage = {12, 8, 5};

    //important objects
    private GameObject player;
    private Player playerScript;
    private Animator animator;
    private UnityEngine.AI.NavMeshAgent agent;

    private Vector3 playerDirection, seenDirection, wanderDest;                                    //player direction, lookdirection, wander destination
    private float visDist = 25f, visAngle = 50f;                                                   //vision distance and vision angle
    private float startShootDist = 15f, stopShootDist = 18f, stopAgentDist = 0.8f;                 //distance to start shooting, distance to stop shooting, distance to stop moving (when arrived close enough to destination)
    private float walkSpeed = 1.5f, runSpeed = 6f;                                                 //running and walking speeds
    private int lookAroundStartTime, lookAroundTotTime = 500;                                      //cooldown for looking around

    private int mask;                        //determines which layers collisions will happen

    //essential player states and information
    private bool seen;
    private int state;
    private int health;
    private bool canShoot;

    //bullet gameobject to instantiate, along with the parent to instantiate in
    [SerializeField]
    private GameObject bullet;
    private Transform bulletParent;

    //last seen object keeping track of the position where the enemy last saw the player (if player is out of sight)
    [SerializeField]
    private GameObject lastSeenObj;
    private Transform lastSeenParent;
    private GameObject lastSeen;
    
    //persuit object to calculate the future position of the player for the enemy to head towards
    //(moves to predicted position of player, rather than directly to player)
    [SerializeField]
    private GameObject persuitDestObj;
    private Transform persuitDestParent;
    private GameObject persuitDest;

    //collision effect
    [SerializeField]
    private GameObject hitMarker;

    //health bar
    private ValueBar healthBar;


    //called once at the beginning
    private void Start() {
        
        //finds and assigns all important components and objects
        player = GameObject.Find("Player");
        playerScript = player.GetComponent<Player>();
        animator = GetComponent<Animator>();
        agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        persuitDestParent = GameObject.Find("PersuitDests").GetComponent<Transform>();
        lastSeenParent = GameObject.Find("LastSeens").GetComponent<Transform>();
        bulletParent = GameObject.Find("Bullets").GetComponent<Transform>();
        healthBar = transform.Find("HealthCanvas").Find("HealthBar").GetComponent<ValueBar>();


        //default settings at the beginning of game
        mask = ~(1 << 6);
        seen = false;
        canShoot = true;
        health = 100;

        //instantiate gameobjects to mark "last seen" position and "persuit" (predicted) player position
        lastSeen = Instantiate(lastSeenObj, lastSeenParent);
        persuitDest = Instantiate(persuitDestObj, persuitDestParent);

        //generate a random destination on the map to wander to
        //start state as wandering
        wanderDest = genEnemyPosition();
        SetState(WANDER);

    }


    //changes enemy state
    private void SetState(int nextState) { 

        if(nextState == CHASE || nextState == FIND) agent.speed = runSpeed;     //running speed if chasing or finding player
        else if(nextState == WANDER) agent.speed = walkSpeed;                   //walking speed if wandering

        animator.SetInteger("state", nextState);                                //update animation given the state
        state = nextState;                                                      //update state value

    }

    //this method is called each frame
    private void Update() {
        
        //calculate direction to where player was last seen, and direction towards player
        seenDirection = lastSeen.transform.position - transform.position;
        playerDirection = player.transform.position - new Vector3(transform.position.x, player.transform.position.y, transform.position.z);

        //drawing lines for debugging purposes
        Vector3 forward = transform.forward * visDist;
        Vector3 newVec = Quaternion.AngleAxis(visAngle, transform.up) * forward;
        Vector3 newVec2 = Quaternion.AngleAxis(-visAngle, transform.up) * forward;
        Debug.DrawLine(transform.position, transform.position + newVec);
        Debug.DrawLine(transform.position, transform.position + newVec2);

        //checks if player is within sight
        CheckSeen();
        
        //state determines what the agent is currently doing
        if(state == WANDER) Wandering();
        else if (state == CHASE) Chasing();
        else if(state == SHOOT) Shooting();
        else if (state == FIND) Finding();
        else if (state == LOOKAROUND) LookingAround();

        if(health <= 0) Kill();

    }

    //this method calculates the vector to go directly towards the player
    private Vector3 CalculateSeek() {
        persuitDest.transform.position = lastSeen.transform.position;
        return lastSeen.transform.position;
    }

    //this method calculates the predicted player position in the future for the enemy to run towards
    private Vector3 CalculatePersuit() {

        float lookAhead = playerDirection.magnitude / (agent.speed + player.GetComponent<Player>().GetSpeed());

        Vector3 newTarget = player.transform.position + player.transform.forward * lookAhead * 3;        
        persuitDest.transform.position = new Vector3(newTarget.x, player.transform.position.y-0.8f, newTarget.z);

        return newTarget;
    }


    //this method checks if player is within sight
    private void CheckSeen() {

        float angle = Vector3.Angle(playerDirection, this.transform.forward);

        //if player is within vision distance of the enemy, and the angle is within vision angle,
        //shoot a ray directly towards the player
        //if the ray hits the player, this means there is nothing obstructing the enemy vision, and the enemy can see the player
        //otherwise, enemy cannot see the player
        if(playerDirection.magnitude < visDist && angle < visAngle) {
            
            Ray visionRay = new Ray(new Vector3(transform.position.x, player.transform.position.y, transform.position.z), playerDirection);
            RaycastHit hitInfo;

            if(Physics.Raycast(visionRay, out hitInfo, Mathf.Infinity, mask)) {

                if(hitInfo.collider.gameObject.name == "Player") seen = true;
                else seen = false;

            } else {
                seen = false;
            }

        }

        //update last seen positions        
        if(seen) lastSeen.transform.position = player.transform.position;
        lastSeen.transform.position = new Vector3(lastSeen.transform.position.x, transform.position.y, lastSeen.transform.position.z);

    }

    //this method generates a position on the map where the enemy may spawn
    private Vector3 genEnemyPosition() {
        
        //possible positions is anywhere on one of two circles
        //randomly pick a circle, and then find a random position on the circle
        //find the closest navMesh area given the random position on the circle
        //this is the chosen position to generate an enemy

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


    private void Wandering() {

        //if enemy sees player, move to chase state
        if(seen) SetState(CHASE);
        else {
            
            //move enemy towards a wandering destination
            //if enemy is close enough to the wandering destination, pick a new destination to wander to
            agent.SetDestination(wanderDest);
            if((agent.transform.position - wanderDest).magnitude < stopAgentDist) wanderDest = genEnemyPosition();

        }
    }

    private void Chasing() {
        
        //if player stopped, move directly towards player
        //if player is moving, calculate the future position of the player and move to that instead
        if(playerScript.isStop()) agent.SetDestination(CalculateSeek());
        else agent.SetDestination(CalculatePersuit()); 

        //if player is out of sight, move to find state
        if(!seen) SetState(FIND);

        //if player is in sight and within shooting range, move to shoot state
        if(seenDirection.magnitude < startShootDist && seen) SetState(SHOOT);

    }

    private void Finding() {
        
        //move directly to last seen position
        agent.SetDestination(CalculateSeek());

        //if player within sight, move to chase scene
        if(seen) SetState(CHASE);

        //if arrived at last seen position, move to look around state
        if((lastSeen.transform.position - transform.position).magnitude < stopAgentDist) {
            lookAroundStartTime = Time.frameCount;
            SetState(LOOKAROUND);
        }
    }


    private void Shooting() {
        
        //look towards player
        seenDirection.y = 0;
        transform.rotation = Quaternion.LookRotation(seenDirection);

        //set destination to where it is already (doesnt move)
        agent.SetDestination(agent.transform.position);

        var shootTime = transform.Find("RigAss/RigSpine1/RigSpine2/RigSpine3/RigArmRightCollarbone/RigArmRight1/RigArmRight2/RigArmRight3");

        //when shooting, it will instantiate a bullet in the gun
        //the gun moves up and down in the animation
        //after the gun reaches a threshold down, call the shoot method

        if(canShoot && shootTime.localEulerAngles.y > 99.05) {
            Shoot();
            canShoot = false;
        
        } else if(!canShoot && shootTime.localEulerAngles.y < 96) canShoot = true;


        //if player out of shoot distance, move to chase state
        if(seenDirection.magnitude > stopShootDist) SetState(CHASE);
        
        //if player out of sight, move to find state
        if(!seen) SetState(FIND);
        
    }
    

    private void LookingAround() {
        
        //if see player, chase player
        if(seen) SetState(CHASE);

        //after a while, if player is still not found, return to wander state
        if(Time.frameCount - lookAroundStartTime >= lookAroundTotTime) SetState(WANDER);

    }



    private void Shoot() {

        canShoot = false;
        StartCoroutine(ShootCoolDown());   //shooting cool down

        //instantiate a new bullet object at the position of the gun
        var bulletPos = transform.Find("RigAss/RigSpine1/RigSpine2/RigSpine3/RigArmRightCollarbone/RigArmRight1/RigArmRight2/RigArmRight3/RigPistolRight/SciFiHandGun(Clone)/GunCylinder");
        var bulletObj = Instantiate(bullet, bulletPos.position, bulletPos.rotation, bulletParent);

        bulletObj.GetComponent<Bullet>().setHitMarker(hitMarker);

    }

    
    IEnumerator ShootCoolDown() {
        yield return new WaitForSeconds(0.5f);
        canShoot = true;

    }
    

    //method called when enemy is shot
    public void Shot(int type, float damageMultiplier) {

        //decreaes health and update healthbar
        health -= (int)(damage[type] * damageMultiplier);
        healthBar.SetValue(health);
    }

    //method called when enemy is killed
    private void Kill() { 

        //destroy all correlated gameobjects
        Destroy(persuitDest);
        Destroy(lastSeen);
        Destroy(this.gameObject); 
    
    }

}
