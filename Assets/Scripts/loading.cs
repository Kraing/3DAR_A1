using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class loading : MonoBehaviour
{
    GameObject Controller;
    private RectTransform rectComponent;
    private float rotateSpeed = 100f;

    // Start is called before the first frame update
    void Start()
    {
        // Get Controller Obj
        Controller = GameObject.Find("Controller");

        // Get object transform property
        rectComponent = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
        bool check = Controller.GetComponent<SceneController>().model_loaded;

        // Check if the data are loaded
        if(check)
        {
            Controller.GetComponent<SceneController>().StartApp();
        }
        
        // If not loaded continue with the gear loading animation
        rectComponent.Rotate(0f, 0f, -rotateSpeed * Time.deltaTime);

    }
}
