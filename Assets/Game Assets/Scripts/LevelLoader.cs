using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelLoader : MonoBehaviour {
    public static bool enableAI = true;
    public static bool enablePlayer2 = true;

    public GameObject aiToggle;
    public GameObject player2Toggle;

    public bool isInLevel = false;
    public GameObject aiPlayer;
    public GameObject secondPlayer;
    public GameObject secondCamera;

    // Use this for initialization
    void Start () {
		if (isInLevel)
        {
            aiPlayer.SetActive(enableAI);
            secondPlayer.SetActive(enablePlayer2);
            secondCamera.SetActive(enablePlayer2);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void loadScene()
    {
        enableAI = aiToggle.GetComponent<Toggle>().isOn;
        enablePlayer2 = player2Toggle.GetComponent<Toggle>().isOn;

        SceneManager.LoadScene("level_garden");
    }
}
