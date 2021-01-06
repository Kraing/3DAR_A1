using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GetController : MonoBehaviour
{

    GameObject Controller;

    Animator transition;


    void Start()
    {
        // Get active scene
        Scene scene = SceneManager.GetActiveScene();

        // Get Controller Obj
        Controller = GameObject.Find("Controller");

        // Get animator for scene transition
        transition = GameObject.Find("Animation_with_rotating_logo").GetComponent<Animator>();

        if(scene.name == "menu")
        {
            Button start_btn = GameObject.Find("start_btn").GetComponent<Button>();
            Button credits_btn = GameObject.Find("credits_btn").GetComponent<Button>(); 
            Button quit_btn = GameObject.Find("quit_btn").GetComponent<Button>(); 

            start_btn.onClick.AddListener(StartFunction);
            credits_btn.onClick.AddListener(CreditsFunction);
            quit_btn.onClick.AddListener(QuitFunction);
        }
        else if(scene.name == "credits")
        {
            Button back_btn = GameObject.Find("back_btn").GetComponent<Button>();
            back_btn.onClick.AddListener(BackMenu);
        }
        else if(scene.name == "application")
        {
            Button back_btn = GameObject.Find("menu_btn").GetComponent<Button>();
            back_btn.onClick.AddListener(BackMenu);
        }
    }


    void StartFunction()
    {
		if(Controller != null)
        {
            float p_model = Controller.GetComponent<SceneController>().progress_model;
            float p_pressure = Controller.GetComponent<SceneController>().progress_pressure;
            float p_flow = Controller.GetComponent<SceneController>().progress_flow;

            // Based on progress bar decide if use loading scene or directly application scene
            if(p_model==100f && p_pressure==100f && p_flow==100f)
                StartCoroutine("ChangeScene", 3);
            else
                StartCoroutine("ChangeScene", 2);
        }
        else
        {
            Debug.Log("Controller Object Not Found.");
        }
	}

    void CreditsFunction()
    {
		if(Controller != null)
        {
            StartCoroutine("ChangeScene", 1);
        }
        else
        {
            Debug.Log("Controller Object Not Found.");
        }
	}

    void QuitFunction()
    {
		if(Controller != null)
        {
            StartCoroutine("ChangeScene", -1);
        }
        else
        {
            Debug.Log("Controller Object Not Found.");
        }
	}

    void BackMenu()
    {
		if(Controller != null)
        {
            StartCoroutine("ChangeScene", 0);
        }
        else
        {
            Debug.Log("Controller Object Not Found.");
        }
	}


    public IEnumerator ChangeScene(int k)
    {
        transition.SetTrigger("triggerino");
        yield return new WaitForSeconds(1f);

        if(k == -1)
        {
            Controller.GetComponent<SceneController>().ExitApp();
        }
        else if(k == 0)
        {
            Controller.GetComponent<SceneController>().BackMainMenu();
        }
        else if(k == 1)
        {
            Controller.GetComponent<SceneController>().CreditsApp();
        }
        else if(k == 2)
        {
            Controller.GetComponent<SceneController>().LoadingApp();
        }
        else if(k == 3)
        {
            Controller.GetComponent<SceneController>().StartApp();
        }
    }
}
