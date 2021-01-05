using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    public Animator transition;

    public float transitionTime = 1f;

    public void LoadNextLevel()
    {
        Debug.Log("prima coroutine");
    	StartCoroutine(LoadLevel());
        Debug.Log("dopo coroutine");
    }

    public IEnumerator LoadLevel()
    {
        Debug.Log("inizio?");
        transition.SetTrigger("triggerino");
        yield return new WaitForSeconds(transitionTime);
        Debug.Log("set?");	
    }
}
