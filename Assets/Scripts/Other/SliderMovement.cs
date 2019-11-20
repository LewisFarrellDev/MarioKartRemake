using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SliderMovement : MonoBehaviour {

     Slider mySlider;
     GameObject thisSlider;
     float sliderChange;
     float maxSliderValue;
     float minSliderValue;
     float sliderRange;
     const float SLIDERSTEP = 100.0f; //used to detrime how fine to change value
     const string SLIDERMOVE = "Horizontal";

    //Initialize values
     void Awake()
    {
        mySlider = GetComponentInParent<Slider>();
        thisSlider = gameObject; //used to deterine when slider has 'focus'
        maxSliderValue = mySlider.maxValue;
        minSliderValue = mySlider.minValue;
        sliderRange = maxSliderValue - minSliderValue;
    }

     void Update()
    {
        //If slider has 'focus'
        if (thisSlider == EventSystem.current.currentSelectedGameObject)
        {
            sliderChange = Input.GetAxis(axisName: SLIDERMOVE) * sliderRange / SLIDERSTEP;
            float sliderValue = mySlider.value;
            float tempValue = sliderValue + sliderChange;
            if (tempValue <= maxSliderValue && tempValue >= minSliderValue)
            {
                sliderValue = tempValue;
            }
            mySlider.value = sliderValue;
        }
    }


}
