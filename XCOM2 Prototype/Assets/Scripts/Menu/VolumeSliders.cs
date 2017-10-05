using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSliders : MonoBehaviour {

    Slider volumeSlider;
    float volumeValue;

    Text volumeSliderValue;
    // Use this for initialization
    void Start () {

        

    }
    private void Awake()
    {
        volumeValue = GetComponentInParent<Slider>().value;
        volumeValue *= 100;
        int temp = (int)volumeValue;

        volumeSliderValue = GetComponent<Text>();


        volumeSliderValue.text = temp.ToString();
    }

    // Update is called once per frame
    void Update () {

    }
    public void UpdateText()
    {
        volumeValue = GetComponentInParent<Slider>().value;
        volumeValue *= 100;
        int temp = (int)volumeValue;

        volumeSliderValue = GetComponent<Text>();


        volumeSliderValue.text = temp.ToString();
    }
}
