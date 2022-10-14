
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor;

public class OpenSkyDataHandler {

    #region Variables and Constructors

    private List<ClientData> clientsData;
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
        clientsData = new List<ClientData>();
    }
    #endregion
 
    #region Public Methods

    public void SetAnotherClientsData(DataPackage<ClientData> dataPackage) {
        if(clientsData.Count > 0)
            clientsData.RemoveAll(client => client.id != OpenSkyClient.Client.id);

        clientsData.AddRange(dataPackage.clientsData);
    }
   
    public void UpdateAnotherClientsByData() {
        OpenSkyWatcher[] watchers = GameObject.FindObjectsOfType<OpenSkyWatcher>();

        foreach (OpenSkyWatcher watcher in watchers)
        {
            foreach(ClientData clientData in clientsData)
            {
                if(clientData.id != watcher.owner)
                {
                    foreach(WatcherData watcherData in clientData.watchersData)
                    {
                        if(watcherData.id == watcher.id)
                            watcher.RefreshByComponentsData(watcherData.componentsData);
                    }
                }
            }
        }
    }

    public void SetOwnClientData() {
        OpenSkyWatcher[] watchers = GameObject.FindObjectsOfType<OpenSkyWatcher>();
        
        List<WatcherData> watchersData = new List<WatcherData>();
        foreach (OpenSkyWatcher watcher in watchers)
        {
            if(watcher.isOwner)
            {
                ComponentData[] componentsData = watcher.GetComponentsData(); 
                WatcherData watcherData = new WatcherData(watcher.id, componentsData);
                watchersData.Add(watcherData);
            }
        } 

        ClientData clientData = new ClientData(OpenSkyClient.Client.id, watchersData.ToArray());

        if(clientsData.Count > 0)
            clientsData.RemoveAll(client => client.id == OpenSkyClient.Client.id);
        clientsData.Add(clientData);
    }

    public ClientData GetOwnClientData() {
        return clientsData.Find(client => client.id == OpenSkyClient.Client.id);
    }
    
    #endregion
}

[System.Serializable]
public class ClientData 
{
    public string id;
    public WatcherData[] watchersData;

    public ClientData(string id, WatcherData[] watchersData) 
    {
        this.id = id;
        this.watchersData = watchersData;
    }
}

[System.Serializable]
public class WatcherData 
{
    public string id;
    public ComponentData[] componentsData;
    public WatcherData(string id, ComponentData[] componentsData) 
    {
        this.id = id;
        this.componentsData = componentsData;
    }
}

[System.Serializable]
public class ComponentData {
    public string assemblyQualifiedName;
    public string data;

    public ComponentData(string assemblyQualifiedName, string data) 
    {
        this.assemblyQualifiedName = assemblyQualifiedName;
        this.data = data;
    }
}

[System.Serializable]
public class DataPackage<T>
{
    public ClientData[] clientsData;
}
