using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UserInterface : MonoBehaviour
{   
    [Header("References")]
    public MazeGenerator maze;
    public GameObject playerPrefab;
    public GameObject viewMode;

    [Header("UI widgets")]
    public GameObject errorPanel;
    public Text errorMessage;
    public GameObject backToViewText;
    public InputField widthField;
    public InputField heigthField;

    private bool error = false;
    private GameObject player;
    
    void Update() {
        if(backToViewText.activeInHierarchy){ // if the text is active
            if(Input.GetKeyDown("escape")){  //press Esc button
                BackToViewMode();
            }
        }
    }

    public void RegenerateMaze(){ // regenerate the maze
        int width;
        int heigth;

        if(int.TryParse(widthField.text, out width) && width > 0 && !error){ // parse the InputField text to an int and check if width is greater than 0 and if error is false
            maze.rows = width;
        }else{
            error = true;
        }

        if(int.TryParse(heigthField.text, out heigth) && heigth > 0 && !error){// parse the InputField text to an int and check if heigth is greater than 0 and if error is false
            maze.columns = heigth;
        }else{
            error = true;
        }

        if(!error){ // create maze
            maze.GenerateMaze();
        }else{ // error message
            openErrorMessage();
        }
    }

    void openErrorMessage(){ // open an error message
        errorPanel.SetActive(true); 
        errorMessage.text = "Width and Heigth are invalid!";
        StartCoroutine(CloseErrorMessage());
    }
    public void Play(){ // play as a first person character in the maze
        if(!error){
            player  = Instantiate(playerPrefab, new Vector3(0, 1.5f, 0), Quaternion.identity);
            viewMode.SetActive(false);
            backToViewText.SetActive(true);
            LeanTween.scale(backToViewText,backToViewText.transform.localScale * 10, 0.5f);
        }
    }
    public void BackToViewMode(){ // quit playing as first person character and go back to viewing the maze
            Destroy(player);
            backToViewText.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f); // change the scale of the text
            backToViewText.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None; // mouse cursor is unlocked
            viewMode.SetActive(true);
    }

    public void BackToMenu(){
        SceneManager.LoadScene("Menu"); // go back to the menu scene
    }
    
    IEnumerator CloseErrorMessage(){ 
    yield return new WaitForSeconds(2); // after 2 seconds close the error
        errorPanel.SetActive(false);
        error = false;
    }
}
