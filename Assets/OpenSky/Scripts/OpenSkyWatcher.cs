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
            if(componentData.type == typeof(Transform))
                EditorJsonUtility.FromJsonOverwrite(componentData.data, gameObject.transform);
            else
                JsonUtility.FromJsonOverwrite(componentData.data, componentData.type);
        }

    }

    void Update() {
        Component[] components = gameObject.GetComponents(typeof(Component));
        OpenSkyDataHandler.Data.SetWatcherComponents(_id, components);
    }

}
