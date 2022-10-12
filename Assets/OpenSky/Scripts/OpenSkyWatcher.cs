using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OpenSkyWatcher : MonoBehaviour
{
    private string _id;

    public string id {
        get { return _id; }
        set {}
    }

    void Awake() {
        _id = System.Guid.NewGuid().ToString();
    }

    public void RefreshComponents() {
        ComponentData[] componentsData = OpenSkyDataHandler.Data.GetWatcherComponentsById(_id);

        foreach(ComponentData componentData in componentsData) {
            System.Type type = System.Type.GetType(componentData.assemblyQualifiedName);

            #if UNITY_EDITOR
                if(type == typeof(Transform))
                    EditorJsonUtility.FromJsonOverwrite(componentData.data, gameObject.transform);
            #endif
        
            if(type != typeof(Transform))
                JsonUtility.FromJsonOverwrite(componentData.data, gameObject.GetComponent(type));
        }
    }
}
