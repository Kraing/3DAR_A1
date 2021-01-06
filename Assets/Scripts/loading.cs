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

        // Get canvas
        canvas = GameObject.Find("Canvas");

        // Get object transform property
        rectComponent = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        // Update progress bars
        p_model = Controller.GetComponent<SceneController>().progress_model;
        p_pressure = Controller.GetComponent<SceneController>().progress_pressure;
        p_flow = Controller.GetComponent<SceneController>().progress_flow;

        // Print the progress to screen
        info_text.GetComponent<Text>().text = "Loading car model " + p_model.ToString("0.00") + "%" + "\n"
                                                + "Loading pressure " +p_pressure.ToString("0.00") + "%" + "\n"
                                                + "Loading flow " + p_flow.ToString("0.00") + "%";
        

        // Check if the data are loaded
        if(p_model == 100f && p_pressure == 100f && p_flow == 100f)
        {
            coroutine = canvas.GetComponent<GetController>().ChangeScene(3);
            StartCoroutine(coroutine);
            
            // Disable the script to avoid multiple load of the scene
            enabled = false;
        }
        
        // If not loaded continue with the gear loading animation
        rectComponent.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);

    }
}
