using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class OpenSkyClient {

    #region Instances and Constructors
    private string _nickname;

    private static OpenSkyClient _instance;
    private static Object _clientHandler;

    public string nickname {
        get {
            return _nickname;
        }
        set {
            if(_clientHandler != null)
                OpenSkyLogger.Error("The nickname cannot be set after connecting to the server.");
            else
                _nickname = value;
        }
    }
    public static OpenSkyClient Client {
        get {
            if(_instance == null) 
                _instance = new OpenSkyClient();

            return _instance;
        }
        set {}
    }
 
    private OpenSkyClient() {
        _nickname = System.Guid.NewGuid().ToString();
    }
    #endregion
 
    #region Public Methods
    public void Connect(Object clientHandler) {
        _clientHandler = clientHandler;

        OpenSkyLogger.Info("Successfully connected to the server.");
        _InvokeClientHandlerCallback("OnConnect");
    }
    #endregion

    #region Private Methods
    private bool _InvokeClientHandlerCallback(string callbackName) {
        Type clientHandlerType = _clientHandler.GetType();
        System.Reflection.MethodInfo callBackMethodInfo = clientHandlerType.GetMethod(callbackName);

        if(callBackMethodInfo != null)
        {
            callBackMethodInfo.Invoke(_clientHandler, null);
            return true;
        }

        return false;
    } 
    #endregion
}
