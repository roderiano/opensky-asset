using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OpenSkyWatcher : MonoBehaviour
{
    private string _id = null;
    private string _owner = null;

    public string id {
        get { return _id; }
        set {}
    }

    public string owner {
        get { return _owner; }
        set {}
    }

    public bool isOwner {
        get { return _owner == OpenSkyClient.Client.id; }
        set {}
    }

    void Awake() {
        _id = System.Guid.NewGuid().ToString();
    }

    void Start() {
        if(_owner == null)
            _owner = OpenSkyClient.Client.id;
    }

    public void RefreshByComponentsData(ComponentData[] componentsData) {
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

    public ComponentData[] GetComponentsData() {
        Component[] components = gameObject.GetComponents(typeof(Component));
        ComponentData[] componentsData = new ComponentData[components.Length - 1];

        int idxComponent = 0;
        foreach(Component component in components) {
            System.Type componentType = component.GetType();
            if(componentType != typeof(OpenSkyWatcher))
            {
            
                if(component.GetType() != typeof(Transform))
                    componentsData[idxComponent] = new ComponentData(componentType.AssemblyQualifiedName, JsonUtility.ToJson(component));
                    
                idxComponent++;
            }
        }

        return componentsData;
    }
}
