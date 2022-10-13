using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chat : MonoBehaviour
{
    public string currentMessage = "";
    void Start()
    {
        OpenSkyClient.Client.nickname = "roderiano";
        OpenSkyClient.Client.Connect(this);
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnDisconnect() {
        
    }
}
