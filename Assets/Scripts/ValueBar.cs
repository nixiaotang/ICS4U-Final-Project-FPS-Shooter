using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ValueBar : MonoBehaviour
{

    //slider for value
    private Slider slider;

    //this method is called once in the beginning
    void Awake() {

        //assign slider component
        slider = GetComponent<Slider>();
    }

    //this method updates the slider value
    public void SetValue(int val) { 
        slider = GetComponent<Slider>();
        slider.value = val; 
    }

}
