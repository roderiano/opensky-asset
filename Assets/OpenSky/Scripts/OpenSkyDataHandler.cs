
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class OpenSkyDataHandler {

    #region Variables and Constructors

    private List<ComponentData> watchersComponentsData;
    private static OpenSkyDataHandler _instance;

    public static OpenSkyDataHandler Data {
        get {
            if(_instance == null) 
                _instance = new OpenSkyDataHandler();

            return _instance;
        }
        set {}
    }
 
    private OpenSkyDataHandler() { 
        watchersComponentsData = new List<ComponentData>();
    }
    #endregion
 
    #region Public Methods
    public void SetWatcherComponents(string skyWatcherId, Component[] components) {
        ComponentData[] componentsData = new ComponentData[components.Length - 1];

        int idxComponent = 0;
        foreach(Component component in components) {
            System.Type componentType = component.GetType();
            if(componentType != typeof(OpenSkyWatcher))
            {
                #if UNITY_EDITOR
                if(component.GetType() == typeof(Transform))
                    componentsData[idxComponent] = new ComponentData(skyWatcherId, componentType.AssemblyQualifiedName, EditorJsonUtility.ToJson(component));
                #endif
            
                if(component.GetType() != typeof(Transform))
                    componentsData[idxComponent] = new ComponentData(skyWatcherId, componentType.AssemblyQualifiedName, JsonUtility.ToJson(component));
                    
                idxComponent++;
            }
        }

        List<ComponentData> tempList = new List<ComponentData>();
        if(tempList.Count > 0)
            tempList.RemoveAll(data => data.skyWatcherId == skyWatcherId);
        tempList.AddRange(componentsData);

        watchersComponentsData = tempList;
    }

    public ComponentData[] GetWatcherComponentsById(string skyWatcherId) {
        return watchersComponentsData.FindAll(data => data.skyWatcherId == skyWatcherId).ToArray();
    }

    public string GetJsonWatchersComponentsData() {
        Wrapper<ComponentData> wrapper = new Wrapper<ComponentData>();
        wrapper.componentsData = watchersComponentsData.ToArray();

        return UnityEngine.JsonUtility.ToJson(wrapper);
    }

    public void SetJsonWatchersComponentsData(string json) {
        Wrapper<ComponentData> wrapper = JsonUtility.FromJson<Wrapper<ComponentData>>(json);

        watchersComponentsData.Clear();
        watchersComponentsData.AddRange(wrapper.componentsData);
    }
   
    public void RefreshAllWatchersComponents() {
        OpenSkyWatcher[] watchers = GameObject.FindObjectsOfType<OpenSkyWatcher>();

        foreach (OpenSkyWatcher watcher in watchers)
            watcher.RefreshComponents();   
    }

    public void SetAllComponentsData() {
        OpenSkyWatcher[] watchers = GameObject.FindObjectsOfType<OpenSkyWatcher>();
        

        foreach (OpenSkyWatcher watcher in watchers)
        {
            Component[] components = watcher.gameObject.GetComponents(typeof(Component));
            OpenSkyDataHandler.Data.SetWatcherComponents(watcher.id, components); 
        } 
    }
    
    #endregion
}

[System.Serializable]
public class ComponentData 
{
    public string skyWatcherId;
    public string assemblyQualifiedName;
    public string data;

    public ComponentData(string skyWatcherId, string assemblyQualifiedName, string data) 
    {
        this.skyWatcherId = skyWatcherId;
        this.assemblyQualifiedName = assemblyQualifiedName;
        this.data = data;
    }
}

[System.Serializable]
public class Wrapper<T>
{
    public ComponentData[] componentsData;
}
