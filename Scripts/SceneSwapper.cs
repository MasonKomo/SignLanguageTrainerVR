using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SceneSwapper : MonoBehaviour
{
    public GestureDetector LeftHand;
    public GestureDetector RightHand;
    public Boolean mainMenu;

    public void ChangeScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (mainMenu)
        {
            if (LeftHand.Recognize().name == "Learn")
            {

                if (RightHand.Recognize().name == "ThumbsUp")
                {
                    ChangeScene("Learn");
                }
                // change color of panel1 here.

            }

            if (LeftHand.Recognize().name == "Practice")
            {

                if (RightHand.Recognize().name == "ThumbsUp")
                {
                    ChangeScene("Practice");
                }


            }

            if (LeftHand.Recognize().name == "Freestyle")
            {

                if (RightHand.Recognize().name == "ThumbsUp")
                {
                    ChangeScene("Freestyle");
                }
            }
        }

        if (!mainMenu)
        {
            if (LeftHand.Recognize().name == "LeftBack" && RightHand.Recognize().name == "RightBack")
            {
                Debug.Log("Found Gesture to return to menu");
                ChangeScene("MainMenu");
            }
        }
        
    }
}
