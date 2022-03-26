using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour {

    //Magic numbers
    private int HEAD = 0, BODY = 1, LEG = 2;


    //Gun stats
    /*
                            HandGun    Rifle
        maxAmmo           |   10    |    5    |
        reloadTime        |  1.5s   |  2.0s   |
        shootTime         |   n/a   |  0.5s   |
        damageMultiplier  |   x1    |   x2    |

    */

    //gun stats
    [SerializeField]
    private int gunType, maxAmmo, curDurability;
    [SerializeField]
    private float reloadTime, shootTime, damageMultiplier;

    //gun objects
    [SerializeField]
    private UIManager uiManager;
    [SerializeField]
    private GameObject hitMarker;
    [SerializeField]
    private GameObject shootSound;
    [SerializeField]
    private Transform soundParent;
    [SerializeField]
    private GameObject bulletEffect;
    [SerializeField]
    private Transform effectsParent;

    //more gun stats
    private int ammoCount;
    private bool isReloading = false;
    private int maxDurability = 20;


    //method called when gun is broken and needs to be hidden
    //turns all the meshrenderers of the gun to false, so it won't be rendered
    private void Hide() {
        if(gunType == 0) {

            foreach(Transform child in transform) {
                child.gameObject.GetComponent<MeshRenderer>().enabled = false;
            }

        } else GetComponent<MeshRenderer>().enabled = false;
    }

    //method called when gun is fixed and needs to be unhidden
    //turns all the meshrenderers of the gun to true, so it will be rendered
    private void Unhide() {
        if(gunType == 0) {
            foreach(Transform child in transform) {
                child.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        } else GetComponent<MeshRenderer>().enabled = true;

    }

    //method called at the beginning
    void Start() {

        //resets gun states
        bulletEffect.SetActive(false);
        ammoCount = maxAmmo;

        uiManager.UpdateAmmo((int)(100*((float)ammoCount / (float)maxAmmo)));
        isReloading = false;
        uiManager.UpdateReloadText(isReloading);

    }

    //when gun is fixed and turned on agin
    void OnEnable() {
        uiManager.UpdateAmmo((int)(100*((float)ammoCount / (float)maxAmmo)));               //upgrade ammo count in UI display
        if(curDurability <= 0) Hide();
    }


    //called when gun is shooting
    public void Shoot() {

        //if ammoCount or durability is 0, don't do anything
        if(ammoCount == 0) return;
        if(curDurability == 0) return;

        //decrease ammo count and gun durability
        ammoCount--;                                                     
        curDurability--;
        
        //update UI for gun durability and ammo
        uiManager.UpdateDurability(curDurability, gunType);
        uiManager.UpdateAmmo((int)(100*((float)ammoCount / (float)maxAmmo)));               //upgrade ammo count in UI display

        if(curDurability <= 0) Hide();

        //shoots a ray from the mouse to the screen
        //if the ray hits an enemy, decrease enemy health depending on where it was shot
        //(y position determines if it's a headshot, bodyshot or legshot)
        Ray rayOrigin = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hitInfo;

        if (Physics.Raycast(rayOrigin, out hitInfo)) {

            if(hitInfo.collider.gameObject.tag == "Enemy") {

                if(hitInfo.point.y > 2) hitInfo.collider.gameObject.GetComponent<Enemy>().Shot(HEAD, damageMultiplier);
                else if (hitInfo.point.y > 1.2) hitInfo.collider.gameObject.GetComponent<Enemy>().Shot(BODY, damageMultiplier);
                else hitInfo.collider.gameObject.GetComponent<Enemy>().Shot(LEG, damageMultiplier);
            }

            //collision effect
            Instantiate(hitMarker, hitInfo.point, Quaternion.LookRotation(hitInfo.normal), effectsParent);

        }
        
        Instantiate(shootSound, soundParent);          //make new sound gameobject to play shooting sound
        StartCoroutine(ShootEffects());                //display shooting visual effects

    }

    //called when gun is reloading
    public void Reload() {

        //if gun still alive and not already reloading,
        //update ui and start reloading time
        if(curDurability > 0 && !isReloading) {            
            isReloading = true;
            uiManager.UpdateReloadText(isReloading);
            StartCoroutine(Reloading());
        }

    }

    //called when gun is fixed
    public void ResetDurability() {

        //reset durability numbers and update UI
        curDurability = maxDurability;
        uiManager.UpdateDurability(curDurability, gunType);
        Unhide();

    }

    public bool GetReloading() { return isReloading; }


    IEnumerator ShootEffects() {
        //set bullet effects active, wait 0.1 secs, set bullet effects inactive
        bulletEffect.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        bulletEffect.SetActive(false);
    }

    IEnumerator Reloading() {

        yield return new WaitForSeconds(reloadTime); //reload cool down, wait 1.5 seconds

        //reset ammo count and display new ammo count in UI
        ammoCount = maxAmmo;
        uiManager.UpdateAmmo((int)(100*((float)ammoCount / (float)maxAmmo)));

        //indicate reloading finished
        isReloading = false;
        uiManager.UpdateReloadText(isReloading);
    }


}
