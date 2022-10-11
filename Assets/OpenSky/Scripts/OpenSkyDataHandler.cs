
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
                componentsData[idxComponent] = new ComponentData(skyWatcherId, componentType, EditorJsonUtility.ToJson(component));
                idxComponent++;
            }
        }

        if(watchersComponentsData.Count > 0)
            watchersComponentsData.RemoveAll(data => data.skyWatcherId == skyWatcherId);
        watchersComponentsData.AddRange(componentsData);
    }

    public ComponentData[] GetWatcherComponents(string skyWatcherId) {
        watchersComponentsData.FindAll(data => data.skyWatcherId == skyWatcherId);
        return watchersComponentsData.ToArray();
    }

    public string GetJsonWatchersComponentsData() {
        Wrapper<ComponentData> wrapper = new Wrapper<ComponentData>();
        wrapper.componentsData = watchersComponentsData.ToArray();

        return UnityEngine.JsonUtility.ToJson(wrapper);
    }
   
    #endregion
}

[System.Serializable]
public class ComponentData 
{
    public string skyWatcherId;
    public System.Type type;
    public string data;

    public ComponentData(string skyWatcherId, System.Type type, string data) 
    {
        this.skyWatcherId = skyWatcherId;
        this.type = type;
        this.data = data;
    }
}

[System.Serializable]
public class Wrapper<T>
{
    public ComponentData[] componentsData;
}
