using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class sliderUI : MonoBehaviour
{
    public Slider rotationSlider;
    public GameManager gameManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void RotatePlayer()
    {
        gameManager.SetPlayerRotation((int)rotationSlider.value);
    }
}
