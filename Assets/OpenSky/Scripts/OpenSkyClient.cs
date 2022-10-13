using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class OpenSkyClient {

    #region Variables and Constructors
    private string _id;
    private string _nickname;


    private static OpenSkyClient _instance;
    private static Object _clientHandler;

    public string nickname {
        get {
            return _nickname;
        }
        set {
            if(OpenSkySocketCom.Socket.isConnected)
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
    }
    #endregion
 
    #region Public Methods
    public void Connect(Object clientHandler) {
        try
        {
            _clientHandler = clientHandler;

            if (_nickname == null || _nickname == string.Empty)
            {
                OpenSkyLogger.Error("The nickname cannot be null or empty string.");
                return;
            }

            if (OpenSkySocketCom.Socket.isConnected)
            {
                OpenSkyLogger.Error("The server is already connected.");
                return;
            }
                
            
            OpenSkySocketCom.Socket.StartCommunication();

        }
        catch (System.Exception e)
        {
            OpenSkyLogger.Error(e.Message);
            _InvokeClientHandlerCallback("OnConnectFail");
            return;
        }

        if(OpenSkySocketCom.Socket.isConnected)
        {
            OpenSkyLogger.Info("Successfully connected to the server.");
            _InvokeClientHandlerCallback("OnConnect");
        }
    }

    public void Disconnect(string reason = "") {
        OpenSkySocketCom.Socket.StopCommunication();
        OpenSkyLogger.Warning(string.Format("Disconnected from the server. {0}", reason));
        _InvokeClientHandlerCallback("OnDisconnect");
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
