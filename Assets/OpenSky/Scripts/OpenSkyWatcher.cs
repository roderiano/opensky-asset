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

    void FixedUpdate() {
        ComponentData[] componentsData = OpenSkyDataHandler.Data.GetWatcherComponents(_id);

        foreach(ComponentData componentData in componentsData) {
            System.Type type = System.Type.GetType(componentData.assemblyQualifiedName);

            #if UNITY_EDITOR
                if(type == typeof(Transform))
                    EditorJsonUtility.FromJsonOverwrite(componentData.data, gameObject.transform);
            #endif
        
            if(type != typeof(Transform))
                JsonUtility.FromJsonOverwrite(componentData.data, componentData.assemblyQualifiedName);
        }

        Component[] components = gameObject.GetComponents(typeof(Component));
        OpenSkyDataHandler.Data.SetWatcherComponents(_id, components);
    }

    void Update() {
        
 
        // string data = OpenSkyDataHandler.Data.GetJsonWatchersComponentsData();
        // Debug.Log(data);
    }

}
