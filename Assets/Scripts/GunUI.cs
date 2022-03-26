using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GunUI : MonoBehaviour
{

    //durability slider
    private Slider slider;
    
    //picture of gun
    [SerializeField]
    private GameObject picture;

    //method called once at start
    void Start() {
        slider = GetComponent<Slider>();

        //if gun is broken, set the active picture to false
        //otherwise, set it to active
        if(slider.value == 0) picture.SetActive(false);
        else picture.SetActive(true);
    }

    //this method updates the durability value and updates the slider
    public void SetValue(int val) { 
        slider.value = val; 

        if(slider.value == 0) picture.SetActive(false);
        else picture.SetActive(true);
    }

}
