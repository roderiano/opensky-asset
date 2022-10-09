using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teste : MonoBehaviour
{
    public void Awake() {
        OpenSkyClient.Client.nickname = "roderiano";
        OpenSkyClient.Client.Connect(this);
    }

    public void OnConnect() {
        Debug.Log("teste connectado");
    }
}
