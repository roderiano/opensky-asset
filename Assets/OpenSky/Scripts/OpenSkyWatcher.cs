using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class OpenSkyWatcher : MonoBehaviour
{
    public string id = null;
    public string owner = null;
    public bool global = false;


    public bool isOwner
    {
        get { return owner == OpenSkyClient.Client.id; }
    }

    void Awake()
    {
        id = System.Guid.NewGuid().ToString();
        global = OpenSkySocketCom.Socket.isConnected ? false : true;
    }

    public void RefreshByComponentsData(ComponentData[] componentsData)
    {
        foreach (ComponentData componentData in componentsData)
        {
            System.Type type = System.Type.GetType(componentData.assemblyQualifiedName);

#if UNITY_EDITOR
            if (type == typeof(Transform))
                EditorJsonUtility.FromJsonOverwrite(componentData.data, gameObject.transform);
#endif

            if (type != typeof(Transform))
                JsonUtility.FromJsonOverwrite(componentData.data, gameObject.GetComponent(type));
        }
    }

    public ComponentData[] GetComponentsData()
    {
        Component[] components = gameObject.GetComponents(typeof(Component));
        ComponentData[] componentsData = new ComponentData[components.Length];

        int idxComponent = 0;
        foreach (Component component in components)
        {
            System.Type componentType = component.GetType();
            if (componentType != typeof(Transform))
                componentsData[idxComponent] = new ComponentData(componentType.AssemblyQualifiedName, JsonUtility.ToJson(component));

            idxComponent++;
        }

        return componentsData;
    }
}
