using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    GameObject Controller;

    GameObject info_text;
    GameObject canvas;
    private RectTransform rectComponent;
    private float rotateSpeed = 100f;

    // Check var
    bool model_check;
    bool pressure_check;
    bool flow_check;

    float p_model;
    float p_pressure;
    float p_flow;

    private IEnumerator coroutine;


    // Start is called before the first frame update
    void Start()
    {
        // Get Controller Obj
        Controller = GameObject.Find("Controller");

        // Get text Obj
        info_text = GameObject.Find("loading_text");

        canvas = GameObject.Find("Canvas");

        // Get object transform property
        rectComponent = GetComponent<RectTransform>();

        /*
        model_check = Controller.GetComponent<SceneController>().load_model;
        pressure_check = Controller.GetComponent<SceneController>().load_pressure;
        flow_check = Controller.GetComponent<SceneController>().load_flow;

        p_model = Controller.GetComponent<SceneController>().progress_model;
        p_pressure = Controller.GetComponent<SceneController>().progress_pressure;
        p_flow = Controller.GetComponent<SceneController>().progress_flow;
        

        // Set startup message
        info_text.GetComponent<Text>().text = "Loading car model ...";
        */
    }

    // Update is called once per frame
    void Update()
    {
        model_check = Controller.GetComponent<SceneController>().load_model;
        pressure_check = Controller.GetComponent<SceneController>().load_pressure;
        flow_check = Controller.GetComponent<SceneController>().load_flow;
        p_model = Controller.GetComponent<SceneController>().progress_model;
        p_pressure = Controller.GetComponent<SceneController>().progress_pressure;
        p_flow = Controller.GetComponent<SceneController>().progress_flow;

        info_text.GetComponent<Text>().text = "Loading car model " + p_model.ToString("0.00") + "%" + "\n"
                                                + "Loading pressure " +p_pressure.ToString("0.00") + "%" + "\n"
                                                + "Loading flow " + p_flow.ToString("0.00") + "%";
        

        // Check if the data are loaded
        if(model_check && pressure_check && flow_check)
        {
            coroutine = canvas.GetComponent<GetController>().ChangeScene(3);
            StartCoroutine(coroutine);
            return;
            //Controller.GetComponent<SceneController>().StartApp();
        }
        
        // If not loaded continue with the gear loading animation
        rectComponent.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);

    }
}
