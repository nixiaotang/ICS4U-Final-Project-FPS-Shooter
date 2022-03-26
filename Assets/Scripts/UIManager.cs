using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class UIManager : MonoBehaviour
{

    //UI objects
    [SerializeField]
    private ValueBar healthBar;
    [SerializeField]
    private GameObject reloadingText;
    [SerializeField]
    private ValueBar ammoBar;

    //gun scripts
    [SerializeField]
    private GunUI handGun;
    [SerializeField]
    private GunUI rifle;


    //methods to update ammo bar, reloading text, health bar and gun durability bar
    public void UpdateAmmo(int count) { ammoBar.SetValue(count); }
    public void UpdateReloadText(bool isReloading) { reloadingText.SetActive(isReloading); }
    public void UpdateHealth(int health) {  healthBar.SetValue(health); }
    public void UpdateDurability(int durability, int gunType) {
        if(gunType == 0) handGun.SetValue(durability);
        else rifle.SetValue(durability);
    }

    //method to update background color of gun UI to indicate the current activated gun
    public void UpdateGun(int gun) {
        
        GameObject handGunAct = transform.Find("Handgun/BackgroundAct").gameObject;
        GameObject rilfeAct = transform.Find("Rifle/BackgroundAct").gameObject;
        
        //depending on the current active gun, set one to active and the othe to inactive
        if(gun == 0) {
            handGunAct.SetActive(true);
            rilfeAct.SetActive(false);
        } else {
            handGunAct.SetActive(false);
            rilfeAct.SetActive(true);
        }

    }
    
}
