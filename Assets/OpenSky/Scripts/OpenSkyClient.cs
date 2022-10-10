using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class OpenSkyClient {

    #region Variables and Constructors
    private string _id;
    private string _nickname;

    private bool _connected;

    private static OpenSkyClient _instance;
    private static Object _clientHandler;

    public string nickname {
        get {
            return _nickname;
        }
        set {
            if(_connected)
                OpenSkyLogger.Error("The nickname cannot be set after connecting to the server.");
            else if (value == null || value == string.Empty)
                OpenSkyLogger.Error("The nickname cannot be null or empty string.");     
            else
                _nickname = value;
        }
    }

    public string id {
        get { return _id; }
        set {}
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
        _id = System.Guid.NewGuid().ToString();
        _connected = false;
    }
    #endregion
 
    #region Public Methods
    public void Connect(Object clientHandler) {
        try
        {
            _clientHandler = clientHandler;

            if (nickname == null || nickname == string.Empty)
                throw new Exception("The nickname cannot be null or empty string.");
        }
        catch (System.Exception e)
        {
            OpenSkyLogger.Error(e.Message);
            _InvokeClientHandlerCallback("OnUnsuccessfulConnect");
            return;
        }

        _connected = true;
        OpenSkyLogger.Info("Successfully connected to the server.");
        _InvokeClientHandlerCallback("OnSuccessfulConnect");
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
