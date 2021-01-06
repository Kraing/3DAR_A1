using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Openpanel : MonoBehaviour
{
	Animator transition;

	public Sprite defaultSprite;
	public Sprite pressedSprite;

	public void TaskOnClick()
	{

		 Button btn = GetComponent<Button>();

		Debug.Log ("You have clicked the button");

		transition = GameObject.Find("Canvas").GetComponent<Animator>();

		if(transition.GetBool("open") == false)
 		{
    		transition.SetBool("open", true);
    		btn.image.sprite =  pressedSprite;
    		return;
 		}
 		else if (transition.GetBool("open") == true)
 		{
    		transition.SetBool("open", false);
    		btn.image.sprite =  defaultSprite;
    		return;
 		}

	}
}
