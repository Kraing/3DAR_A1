using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[System.Serializable]
public class SliderScript : MonoBehaviour
{
    [SerializeField] GameObject flow;
    Slider slider;

    // Start is called before the first frame update
    void Start()
    {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate {UpdateSpeed();});
    }

    // Update the waitTime to change particle animation speed
    void UpdateSpeed()
    {
        flow.GetComponent<Flow>().waitTime = Mathf.Log(slider.value);
    }
}
