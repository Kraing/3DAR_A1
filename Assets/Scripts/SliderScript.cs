using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderScript : MonoBehaviour
{
    Slider slider;
    GameObject flow;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        flow = GameObject.Find("FlowMesh");
        slider.onValueChanged.AddListener(delegate {UpdateSpeed();});
    }

    // Update the waitTime to change particle animation speed
    void UpdateSpeed()
    {
        flow.GetComponent<Flow>().waitTime = slider.value;
    }
}
