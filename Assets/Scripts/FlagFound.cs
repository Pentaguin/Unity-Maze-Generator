using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagFound : MonoBehaviour
{   
    public GameObject flagFoundEffect;
    
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player"){ // player has entered the box colider
            GameObject effect = Instantiate(flagFoundEffect,other.transform.position,Quaternion.identity); //instantiate particle effect
            FindObjectOfType<UserInterface>().backToViewText.SetActive(false);
            StartCoroutine(flagIsFound(effect));
        }
    }

    IEnumerator flagIsFound(GameObject effect){
        yield return new WaitForSeconds(10f); //after 10 seconds destoy the particle system effect and go back to viewMode
        Destroy(effect); 
        FindObjectOfType<UserInterface>().BackToViewMode(); 
    }
}
