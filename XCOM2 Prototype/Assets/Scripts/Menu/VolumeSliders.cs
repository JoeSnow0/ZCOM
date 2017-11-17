using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class VolumeSliders : MonoBehaviour {

    public Slider volumeSlider;
    [SerializeField]
    public float volumeValue;
    static MusicController musicController;
    Text volumeSliderValue;
    
    // Use this for initialization
    void Start () {
        

    }
    private void Awake()
    {
        volumeValue = PlayerPrefs.GetFloat("MasterVolume");
        volumeValue *= 100;
        int temp = (int)volumeValue;

        //GetComponentInParent<Slider>().value;

        volumeSliderValue = GetComponent<Text>();
        volumeSliderValue.text = temp.ToString();

        //Debug.Log(PlayerPrefs.GetFloat("volume"));

        musicController = FindObjectOfType<MusicController>();
        volumeSlider.GetComponent<Slider>().value = PlayerPrefs.GetFloat("MasterVolume");

    


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
