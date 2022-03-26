using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

    //Magic numbers
    private int HANDGUN = 0, RIFLE = 1;

    //to control player
    private static CharacterController controller;

    [SerializeField]
    private float fov;
    private static float runSpeed = 7, walkSpeed = 4;          //walk and run speed
    private static float speed;                                //current speed (either walking or running)
    private static float gravity = 9.81f;
    private static bool isStopped = true;                      //if player is stopped

    private bool canShoot = true;                              //if player can shoot

    private int health;
    private int maxHealth = 100;
    [SerializeField]
    public float mouseSensitivity;

    [SerializeField]
    private UIManager uiManager;

    //current used gun & gun array for the two possible guns
    [SerializeField]
    private int curGun;
    [SerializeField]
    private GameObject []guns;


    //called once at the start
    private void Start() {
        controller = GetComponent<CharacterController>();

        //hide cursor and confine to screen
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;

        //reset basic settings
        health = maxHealth;
        mouseSensitivity = 0.7f;
        fov = 100;
        speed = walkSpeed;

        //start with handgun
        ChangeGun(HANDGUN);

    }

    public float GetSpeed() { return speed; }
    public bool isStop() { return isStopped; }

    //method called when changing guns
    private void ChangeGun(int gun) {

        //if current gun is not reloading
        //set current gun to false, change gun, set new gun to true
        //update gun in UI
        if(!guns[curGun].GetComponent<Gun>().GetReloading()) {
            guns[curGun].SetActive(false);
            curGun = gun;
            guns[curGun].SetActive(true);

            uiManager.UpdateGun(curGun);
        }
    }

    //method called at each frame
    private void Update() {

        Camera.main.fieldOfView = fov;

        //move player
        Move();

        //mouse pressed down (left button)
        if (Input.GetMouseButton(0)) {
            
            //hide the cursor and confine it to the game screen
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Confined;
            mouseSensitivity = 0.7f;
            
            //if player can shoot, then shoot
            if(canShoot) {
                guns[curGun].GetComponent<Gun>().Shoot();
                canShoot = false; //prevents multiple shooting when pressing down
            } 
            
        } else canShoot = true; //only after releasing the mouse button, can the player shoot again


        //reload
        if(Input.GetKeyDown(KeyCode.R)) {
            guns[curGun].GetComponent<Gun>().Reload();
        }

        //change gun
        if(Input.GetKeyDown(KeyCode.F)) {
            if(curGun == HANDGUN) ChangeGun(RIFLE);
            else ChangeGun(HANDGUN);
        }

        //run
        if(Input.GetKey(KeyCode.Space)) speed = runSpeed;
        else speed = walkSpeed;


        //if ESCAPE key is pressed, make cursor visible and not confined
        if (Input.GetKeyDown(KeyCode.Escape)) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            mouseSensitivity = 0;
        }
        
    }


    private void Move() {
        
        //get keyboard input to calculate direction
        Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //calculate velocity with direction, speed and gravity
        Vector3 velocity = direction * speed;
        velocity.y -= gravity;
        velocity = transform.transform.TransformDirection(velocity);

        //move player
        controller.Move(velocity * Time.deltaTime);

        //update "isStopped" variable
        if(velocity.x == 0 && velocity.z == 0) isStopped = true;
        else isStopped = false;

    }

    //method called when player gets hit
    public void Hit() {

        //decrease health and update UI
        health = Mathf.Max(health-5, 0);
        uiManager.UpdateHealth(health);

        //if dead,
        //make cursor visible again and stop confining it within the game screen
        //change to deathscene
        if(health == 0) {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(2);
        }

    }

    //methods when collecting a new gun to reset its durability
    public void CollectRifle() { guns[RIFLE].GetComponent<Gun>().ResetDurability(); }
    public void CollectHandGun() { guns[HANDGUN].GetComponent<Gun>().ResetDurability(); }

    //method when collecting a health powerup
    public void GainHealth() { 

        //increase health and upate UI
        health = Mathf.Min(health+30, 100);
        uiManager.UpdateHealth(health);
    }

}
