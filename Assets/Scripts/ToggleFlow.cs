using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleFlow : MonoBehaviour
{
    Toggle flow_toggle;
    GameObject flow;

    void Start()
    {
        flow_toggle = GetComponent<Toggle>();
        flow = GameObject.Find("FlowMesh");
        flow_toggle.onValueChanged.AddListener(delegate {ToggleValueChanged(flow_toggle);});
    }

    void ToggleValueChanged(Toggle change)
    {
        if(change.isOn)
        {
            flow.SetActive(true);
            // Stop the particle animation
            GameObject.Find("FlowMesh").GetComponent<ParticleSystem>().Stop();
        }
        else
            flow.SetActive(false);

    }
}
