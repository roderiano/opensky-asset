using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OpenSkyWatcher : MonoBehaviour
{
    public string _owner;
    void Awake()
    {
        if(_owner == null)
            _owner = OpenSkyClient.Client.id;
    }

    void Update()
    {
        GetGameObjectState();
    }

    private Dictionary<System.Type, string> GetGameObjectState() {
        Dictionary<System.Type, string> goState = new Dictionary<System.Type, string>();
        Component[] components = gameObject.GetComponents(typeof(Component));
        

        foreach(Component component in components) {
            System.Type componentType = component.GetType();
            if(componentType != typeof(OpenSkyWatcher))
            {
                if(goState.ContainsKey(componentType))
                    goState.Remove(componentType);

                goState.Add(componentType, EditorJsonUtility.ToJson(component));
            }
        }

        return goState;
    }
}
