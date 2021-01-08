using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleModel : MonoBehaviour
{

    Toggle model_toggle;
    GameObject model;

    // Start is called before the first frame update
    void Start()
    {
        model_toggle = GetComponent<Toggle>();
        model = GameObject.Find("ModelMesh");
        model_toggle.onValueChanged.AddListener(delegate {ToggleValueChanged(model_toggle);});
    }

    void ToggleValueChanged(Toggle change)
    {
        if(change.isOn)
            model.SetActive(true);
        else
            model.SetActive(false);
    }
}
