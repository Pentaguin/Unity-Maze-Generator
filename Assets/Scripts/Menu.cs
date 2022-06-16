using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject title;

    void Start(){
        LeanTween.scale(title, title.transform.localScale * 10, 1); //scale the title by 10 times
    }

    public void Play(){
        SceneManager.LoadScene("Maze"); // load to next scene
    }
    public void Quit(){ 
        Application.Quit(); // quit application
    }
    
}
