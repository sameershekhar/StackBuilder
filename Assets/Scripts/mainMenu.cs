using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class mainMenu : MonoBehaviour {

	public Text highScore;
	// Use this for initialization
	void Start () {
		highScore.text = "HighScore :"+ PlayerPrefs.GetInt ("score").ToString();
	}
	
	public void ToGame()
	{
		SceneManager.LoadScene ("game");
	}
}
