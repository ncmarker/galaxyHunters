using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class Directions : MonoBehaviour
{
    private HomeManager homeManager;

    // called each time the DirectionsUI is set to active
    void OnEnable()
    {
        homeManager = GameObject.Find("HomeManager").GetComponent<HomeManager>();
    }

    // waits for both players to decide before unpausing game 
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            homeManager.CloseDirections();
        }        
    }     
}
