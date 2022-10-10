using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teste : MonoBehaviour
{
    public void Awake() {
        OpenSkyClient.Client.nickname = "roderiano";
        OpenSkyClient.Client.Connect(this);
    }

    public void OnSuccessfulConnect() {
        Debug.Log("sucesso na conexao");
    }

    public void OnUnsuccessfulConnect() {
        Debug.Log("falha na conexao");
    }
}
