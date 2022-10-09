using UnityEngine;

public class OpenSkyLogger
{
    private static OpenSkyLogger _instance;
    
    public static OpenSkyLogger Client {
        get {
          if(_instance == null) 
            _instance = new OpenSkyLogger();
    
          return _instance;
        }
        set {}
    }
 
    private OpenSkyLogger() {}

    public static void Info(string message) {
        message = string.Format("<color=cyan><b>[OpenSky]</b></color> {0}", message);
        Debug.Log(message);
    }
    public static void Error(string message) {
        message = string.Format("<color=red><b>[OpenSky]</b></color> {0}", message);
        Debug.LogError(message);
    }
    public static void Warning(string message) {
        message = string.Format("<color=yeallow><b>[OpenSky]</b></color> {0}", message);
        Debug.LogWarning(message);
    }
}
