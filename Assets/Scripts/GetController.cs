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
            Button settings_btn = GameObject.Find("ModelSettings_Button").GetComponent<Button>();
            Button model_bw = GameObject.Find("Show_BW").GetComponent<Button>();
            Button model_color = GameObject.Find("Show_Gradient").GetComponent<Button>();
            Button flow_bw = GameObject.Find("Show_BWF").GetComponent<Button>();
            Button flow_color = GameObject.Find("Show_GradientF").GetComponent<Button>();


            back_btn.onClick.AddListener(BackMenu);
            settings_btn.onClick.AddListener(SettingsTrigger);
            model_bw.onClick.AddListener(ShowBW_M);
            model_color.onClick.AddListener(ShowColor_M);
            flow_bw.onClick.AddListener(ShowBW_F);
            flow_color.onClick.AddListener(ShowColor_F);
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


    void SettingsTrigger()
    {
        Animator slider = GameObject.Find("ModelSettings").GetComponent<Animator>();
        GameObject settings_btn = GameObject.Find("ModelSettings_Button");

        if(slider.GetBool("slide") == false)
        {
            // trigger slide-in animation and change button sprite
            slider.SetBool("slide", true);
            settings_btn.GetComponent<Image>().sprite = settings_btn.GetComponent<SettingsButton>().open;
            return;
        }
        else if(slider.GetBool("slide") == true)
        {
            // trigger slide-out animation and change button sprite
            slider.SetBool("slide", false);
            settings_btn.GetComponent<Image>().sprite = settings_btn.GetComponent<SettingsButton>().normal;
            return;
        }
    }


    void ShowBW_M()
    {
        Toggle model_toggle = GameObject.Find("Toggle_Model").GetComponent<Toggle>();
        if(model_toggle.isOn)
        {
            IEnumerator tmp = LoadMesh("Model", 0);
            StartCoroutine(tmp);
        }
        else
        {
            return;
        }
    }


    void ShowColor_M()
    {
        Toggle model_toggle = GameObject.Find("Toggle_Model").GetComponent<Toggle>();
        if(model_toggle.isOn)
        {
            IEnumerator tmp = LoadMesh("Model", 1);
            StartCoroutine(tmp);
        }
        else
        {
            return;
        }
    }


    void ShowBW_F()
    {
        Toggle flow_toggle = GameObject.Find("Toggle_Flow").GetComponent<Toggle>();
        if(flow_toggle.isOn)
        {
            IEnumerator tmp = LoadMesh("Flow", 0);
            StartCoroutine(tmp);
        }
        else
        {
            return;
        }
    }


    void ShowColor_F()
    {
        Toggle flow_toggle = GameObject.Find("Toggle_Flow").GetComponent<Toggle>();
        if(flow_toggle.isOn)
        {
            IEnumerator tmp = LoadMesh("Flow", 1);
            StartCoroutine(tmp);
        }
        else
        {
            return;
        }
    }


    public IEnumerator LoadMesh(string name, int i)
    {
        transition.SetTrigger("triggerino");
        yield return new WaitForSeconds(1f);
        if(name == "Model")
        {
            GameObject model = GameObject.Find("ModelMesh");
            model.GetComponent<Model>().CreateMesh(i);
        }
        else if(name == "Flow")
        {
            GameObject model = GameObject.Find("FlowMesh");
            model.GetComponent<Flow>().CreateMesh(i);
        }
        transition.SetTrigger("triggerino");
    }


    public IEnumerator ChangeScene(int k)
    {
        transition.SetTrigger("triggerino");
        yield return new WaitForSeconds(.61f);

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
