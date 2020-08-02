using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class main : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI() 
    {
        if (GUI.Button(new Rect((Screen.width/2)-75,(Screen.height/2)-50 , 150, 100), "Başla"))
        {
             SceneManager.LoadScene("game");
        }    
    }
}
