using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFlowAnimation : MonoBehaviour
{
    Toggle animation_toggle;
    GameObject flow;

    // Start is called before the first frame update
    void Start()
    {
        animation_toggle = GetComponent<Toggle>();
        flow = GameObject.Find("FlowMesh");
        animation_toggle.onValueChanged.AddListener(delegate {ToggleValueChanged(animation_toggle);});
    }

    void ToggleValueChanged(Toggle change)
    {
        if(change.isOn)
        {
            flow.GetComponent<ParticleSystem>().Play();
        }
        else
        {
            flow.GetComponent<ParticleSystem>().Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);;
        }
    }
}
