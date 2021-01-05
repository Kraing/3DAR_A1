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
    	StartCoroutine(LoadLevel());
    }

    IEnumerator LoadLevel()
    {
    	transition.SetTrigger("start");

    	yield return new WaitForSeconds(transitionTime);

    	Debug.Log("Load Level FInished");
    }
}
