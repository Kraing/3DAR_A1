using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
 
public class SceneController : MonoBehaviour
{
	// Menu scene swithcer
	public void StartApp()
    {
    	SceneManager.LoadScene("application");
    }
    
    public void CreditsApp()
    {
    	SceneManager.LoadScene("credits");
    }

    public void ExitApp()
    {
    	Debug.Log("exit");  
        Application.Quit(); 
    }

    // Credits scene switcher
    public void BackMainMenu()
    {
    	Debug.Log("menu");  
        SceneManager.LoadScene("menu");
    }

    /*
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("application");
    }
    /*
    // Update is called once per frame
    void Update()
    {
        
    }*/
}
