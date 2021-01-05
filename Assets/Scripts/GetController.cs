using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GetController : MonoBehaviour
{
    // Start is called before the first frame update
    //public GameObject back_btn;
    GameObject Controller;
    GameObject LevelLoader;

    void Start()
    {
        // Get active scene
        Scene scene = SceneManager.GetActiveScene();

        // Get Controller Obj
        Controller = GameObject.Find("Controller");
        LevelLoader = GameObject.Find("LevelLoader");

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
            Debug.Log("MenuReceived");
        }
    }


    void StartFunction()
    {
		if(Controller != null)
        {
            Controller.GetComponent<SceneController>().StartApp();
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
            //LevelLoader.GetComponent<LevelLoader>().LoadNextLevel();
            Controller.GetComponent<SceneController>().CreditsApp();
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
            Controller.GetComponent<SceneController>().ExitApp();
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
            LevelLoader.GetComponent<LevelLoader>().LoadNextLevel();
            Controller.GetComponent<SceneController>().BackMainMenu();
        }
        else
        {
            Debug.Log("Controller Object Not Found.");
        }
	}
}
